# InstagramClone.Gateway

API Gateway using YARP (Yet Another Reverse Proxy) for the InstagramClone microservices architecture.

## Purpose

The gateway acts as a single entry point for all client requests, routing them to the appropriate backend services:
- `/api/*` → InstagramClone.Api (Port 5203)
- `/hubs/*` → InstagramClone.RealTime (Port 5204) - *Coming soon*
- `/storage/*` → InstagramClone.Storage (Port 5205) - *Coming soon*

## Running the Gateway

### Standalone
```bash
cd InstagramClone.Gateway
dotnet run
```
Gateway will start on: `http://localhost:5200`

### With Watch Mode
```bash
dotnet watch run
```

### With All Services
Use VS Code task: **"Run All with Gateway (watch)"**

This will start:
1. Gateway (Port 5200)
2. API (Port 5203)
3. Client (Port 5245)
4. Tailwind CSS watcher

## Architecture

```
Client (5245)
    ↓
Gateway (5200) ← Single entry point
    ↓
    ├─→ API (5203)         [/api/*]
    ├─→ RealTime (5204)    [/hubs/*]   (Future)
    └─→ Storage (5205)     [/storage/*] (Future)
```

## Configuration

### Routes (appsettings.json)

```json
{
  "ReverseProxy": {
    "Routes": {
      "api-route": {
        "ClusterId": "api-cluster",
        "Match": { "Path": "/api/{**catch-all}" }
      }
    },
    "Clusters": {
      "api-cluster": {
        "Destinations": {
          "destination1": { "Address": "http://localhost:5203" }
        }
      }
    }
  }
}
```

### CORS

Configured to allow:
- Client dev servers: `http://localhost:5245`, `https://localhost:7245`
- All HTTP methods
- All headers
- Credentials (for SignalR)

## Features

✅ **Request Routing**: Intelligent path-based routing to services  
✅ **CORS Handling**: Centralized CORS configuration  
✅ **Health Check**: `/health` endpoint for monitoring  
🔜 **Load Balancing**: Multiple destinations per cluster  
🔜 **Rate Limiting**: Protect backend services  
🔜 **Authentication**: JWT validation at gateway level  
🔜 **Request Logging**: Centralized request/response logging  

## Testing the Gateway

### 1. Start Gateway + API
```bash
# Terminal 1 - Gateway
cd InstagramClone.Gateway
dotnet run

# Terminal 2 - API
cd InstagramClone.Api
dotnet run
```

### 2. Test Health Check
```bash
curl http://localhost:5200/health
# Response: {"status":"healthy","service":"gateway"}
```

### 3. Test API Routing
```bash
# This request goes through gateway to API
curl http://localhost:5200/api/auth/profile
```

### 4. Update Client
The client now points to the gateway:
```csharp
BaseAddress = new Uri("http://localhost:5200")
```

## Benefits

1. **Single Entry Point**: Client only needs to know gateway URL
2. **Service Discovery**: Easy to add/remove backend services
3. **Load Balancing**: Distribute requests across multiple instances
4. **Security**: Centralized authentication and authorization
5. **Monitoring**: Single place for logging and metrics
6. **Versioning**: Support API versioning (e.g., `/api/v1`, `/api/v2`)

## Adding New Services

To route to a new service (e.g., RealTime):

1. **Add route in appsettings.json**:
```json
"realtime-route": {
  "ClusterId": "realtime-cluster",
  "Match": { "Path": "/hubs/{**catch-all}" }
}
```

2. **Add cluster**:
```json
"realtime-cluster": {
  "Destinations": {
    "destination1": { "Address": "http://localhost:5204" }
  }
}
```

3. **Done!** Gateway automatically picks up the configuration.

## Next Steps

- [ ] Add InstagramClone.RealTime service
- [ ] Add InstagramClone.Storage service
- [ ] Implement rate limiting
- [ ] Add JWT authentication middleware
- [ ] Add request/response logging
- [ ] Add metrics and monitoring (Prometheus)

## References

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Configuration Reference](https://microsoft.github.io/reverse-proxy/articles/config-files.html)

