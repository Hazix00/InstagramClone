# Security Configuration

## Gateway Authentication

The API is protected by gateway authentication, ensuring that **only the Gateway** can access backend services directly.

## How It Works

```
┌─────────────────────────────────────────────────────────┐
│ Client                                                  │
│  ↓ Makes request to Gateway                            │
├─────────────────────────────────────────────────────────┤
│ Gateway (Port 5200)                                     │
│  ↓ Adds X-Gateway-Key header                           │
├─────────────────────────────────────────────────────────┤
│ API (Port 5203)                                         │
│  ✓ Validates X-Gateway-Key                             │
│  ✓ Processes request                                    │
└─────────────────────────────────────────────────────────┘

❌ Direct API Access (Port 5203)
   → 403 Forbidden: "Direct access forbidden"
```

## Configuration

### 1. Shared Secret Key

Both Gateway and API must have the same key:

**InstagramClone.Gateway/appsettings.json**
```json
{
  "Gateway": {
    "ApiKey": "your-secret-gateway-key-change-in-production"
  }
}
```

**InstagramClone.Api/appsettings.json**
```json
{
  "Gateway": {
    "ApiKey": "your-secret-gateway-key-change-in-production"
  }
}
```

### 2. Gateway Middleware

**Gateway** adds the key to all outgoing requests:
- `GatewayKeyMiddleware` appends `X-Gateway-Key` header
- Transparent to backend services

### 3. API Middleware

**API** validates incoming requests:
- `GatewayAuthenticationMiddleware` checks for valid `X-Gateway-Key`
- Rejects requests without valid key (403 Forbidden)
- Allows `/health` endpoint without key

## Testing

### ✅ Through Gateway (Works)
```bash
curl http://localhost:5200/api/auth/profile
# Response: 200 OK (if authenticated) or 401 Unauthorized
```

### ❌ Direct to API (Blocked)
```bash
curl http://localhost:5203/api/auth/profile
# Response: 403 Forbidden
# {
#   "error": "Direct access forbidden. Please use the API gateway.",
#   "gateway": "http://localhost:5200"
# }
```

### ✅ Health Check (Always Works)
```bash
curl http://localhost:5203/health
# Response: 200 OK (health checks bypass gateway authentication)
```

## Production Best Practices

### 1. Generate Strong API Key

```bash
# Generate a secure random key
openssl rand -base64 64

# Or use C#
using System.Security.Cryptography;
Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
```

### 2. Use Environment Variables

Don't commit keys to source control!

**appsettings.Production.json**
```json
{
  "Gateway": {
    "ApiKey": "${GATEWAY_API_KEY}"  // Read from environment
  }
}
```

**Set environment variable:**
```bash
# Linux/Mac
export GATEWAY_API_KEY="your-actual-secure-key-here"

# Windows
setx GATEWAY_API_KEY "your-actual-secure-key-here"

# Docker
docker run -e GATEWAY_API_KEY="..." myapp
```

### 3. Network-Level Security

In addition to API keys, use network security:

**Docker Compose:**
```yaml
version: '3.8'
services:
  gateway:
    ports:
      - "5200:80"  # Exposed to public
    networks:
      - frontend
      - backend
  
  api:
    # No ports exposed! Only accessible via internal network
    networks:
      - backend  # Private network
```

**Kubernetes:**
```yaml
apiVersion: v1
kind: Service
metadata:
  name: api
spec:
  type: ClusterIP  # Internal only
  ports:
    - port: 80
---
apiVersion: v1
kind: Service
metadata:
  name: gateway
spec:
  type: LoadBalancer  # Public
  ports:
    - port: 80
```

### 4. Rate Limiting

Add rate limiting to the Gateway to prevent abuse:

```csharp
// Gateway - Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
    });
});
```

## Security Layers

This setup provides multiple security layers:

1. **Network Isolation**: API not exposed to public (production)
2. **Gateway Authentication**: Shared secret key validation
3. **JWT Authentication**: User authentication (already implemented)
4. **CORS**: Only allowed origins can make requests
5. **HTTPS**: Encrypted communication (production)

## Troubleshooting

### Client Gets 403 Forbidden

**Problem**: Client is trying to access API directly
**Solution**: Update client to use Gateway URL (http://localhost:5200)

```csharp
// ❌ Wrong - Direct to API
BaseAddress = new Uri("http://localhost:5203")

// ✅ Correct - Through Gateway
BaseAddress = new Uri("http://localhost:5200")
```

### Gateway Key Mismatch

**Problem**: Different keys in Gateway and API
**Solution**: Ensure both `appsettings.json` files have the same `Gateway:ApiKey`

### Development Mode

For local development, the API allows all requests if no key is configured and environment is "Development". For production, the key is **required**.

## Future Enhancements

- [ ] Mutual TLS (mTLS) between Gateway and API
- [ ] JWT validation at Gateway level (validate once at edge)
- [ ] Request signing for tamper detection
- [ ] IP whitelist for Gateway
- [ ] Certificate-based authentication

## References

- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [Defense in Depth](https://en.wikipedia.org/wiki/Defense_in_depth_(computing))
- [OWASP API Security](https://owasp.org/www-project-api-security/)

