# Authentication & Authorization Setup

## Overview

This application uses **JWT (JSON Web Token)** authentication with role-based authorization.

## Architecture

```
Browser (Blazor WASM)
    ↓ [JWT Token in Header]
API Gateway (YARP)
    ↓ [JWT Token + X-Gateway-Key]
API (ASP.NET Core)
    ↓ [JWT Validation + Gateway Auth]
Database (PostgreSQL)
```

## Protected Routes

### Client-Side Protection

Pages with `@attribute [Authorize]` require authentication:

| Page | Route | Protected | Description |
|------|-------|-----------|-------------|
| Home | `/` | ❌ No | Public welcome + authenticated feed |
| Login | `/login` | ❌ No | Login form |
| Register | `/register` | ❌ No | Registration form |
| Forgot Password | `/forgot-password` | ❌ No | Password recovery |
| Verify Email | `/verify-email` | ❌ No | Email verification |
| Profile | `/profile/{username?}` | ✅ Yes | User profile |
| Settings | `/settings` | ✅ Yes | Account settings |
| New Post | `/new-post` | ✅ Yes | Create post |

### How It Works

1. **Unauthenticated Access**: User tries to access `/settings`
2. **Redirect**: `<AuthorizeRouteView>` detects no authentication
3. **RedirectToLogin**: User is sent to `/login`
4. **Post-Login**: User is redirected back to the original page (return URL)

## API Protection

### JWT Authentication

All API endpoints are protected except:
- `/api/auth/login`
- `/api/auth/register`
- `/api/auth/forgot-password`
- `/api/auth/reset-password`
- `/api/auth/send-verification-email`
- `/api/auth/verify-email`
- `/health`

Controllers use `[Authorize]` attribute:
```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    // All actions require authentication
}
```

### Gateway Authentication

The API validates requests come through the Gateway using `X-Gateway-Key` header:

```csharp
// GatewayAuthenticationMiddleware
if (!context.Request.Headers.TryGetValue("X-Gateway-Key", out var providedKey) 
    || providedKey != expectedKey)
{
    return 403 Forbidden;
}
```

**Direct API access is blocked** - all requests must go through the Gateway.

## Authentication Flow

### 1. Login

```
Client (Blazor) → Gateway → API
POST /api/auth/login
{
  "emailOrUsername": "user@example.com",
  "password": "Pa$$w0rd"
}

Response:
{
  "token": "eyJhbGci...",
  "expiresAt": "2025-01-01T00:00:00Z"
}
```

### 2. Token Storage

```javascript
// Stored in browser's localStorage
localStorage.setItem('authToken', token);
```

### 3. Authenticated Requests

```
Client adds Authorization header:
GET /api/posts/feed
Headers:
  Authorization: Bearer eyJhbGci...

Gateway adds X-Gateway-Key:
GET http://localhost:5203/api/posts/feed
Headers:
  Authorization: Bearer eyJhbGci...
  X-Gateway-Key: your-secret-gateway-key-change-in-production

API validates both:
  1. JWT token (user identity)
  2. Gateway key (request origin)
```

## JWT Token Structure

```json
{
  "nameid": "1",
  "unique_name": "john_doe",
  "email": "john@example.com",
  "jti": "cd71be89-2156-41ee-85e9-a921bc672419",
  "nbf": 1761334183,
  "exp": 1761420583,
  "iat": 1761334183,
  "iss": "InstagramCloneApi",
  "aud": "InstagramCloneClient"
}
```

### Claims Used

- `nameid` - User ID
- `unique_name` - Username
- `email` - User email
- `exp` - Expiration timestamp (24 hours)

## Authorization Patterns

### In Controllers

```csharp
// Get current user ID from JWT claims
var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
if (!int.TryParse(userIdClaim, out int userId))
    return Unauthorized();

// Use userId for operations
var posts = await _postService.GetUserPostsAsync(userId);
```

### In Blazor Pages

```razor
<AuthorizeView>
    <Authorized>
        <p>Welcome, @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>Please <a href="/login">log in</a>.</p>
    </NotAuthorized>
</AuthorizeView>
```

## CustomAuthStateProvider

Manages authentication state in Blazor:

```csharp
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    // Loads token from localStorage
    // Parses JWT claims
    // Notifies Blazor of auth state changes
}
```

### Key Methods

- `GetAuthenticationStateAsync()` - Gets current auth state
- `MarkUserAsAuthenticated(token)` - After login
- `MarkUserAsLoggedOut()` - After logout

## Security Configuration

### JWT Settings (appsettings.json)

```json
{
  "Jwt": {
    "Key": "ThisIsASecretKeyForJWTTokenGenerationWithAtLeast32Characters!",
    "Issuer": "InstagramCloneApi",
    "Audience": "InstagramCloneClient"
  }
}
```

⚠️ **Change the Key in production!**

### Gateway Settings

```json
{
  "Gateway": {
    "ApiKey": "your-secret-gateway-key-change-in-production"
  }
}
```

⚠️ **Must match between Gateway and API!**

## CORS Configuration

Configured at the **Gateway level only**:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5245",
      "https://localhost:7245"
    ]
  }
}
```

API has no CORS since it only receives server-to-server requests from the Gateway.

## Troubleshooting

### 401 Unauthorized

**Symptoms**: API returns 401 for authenticated requests

**Common Causes**:
1. Token expired (check `exp` claim)
2. Token not sent (check Authorization header)
3. Invalid signature (check JWT Key matches)
4. Token malformed (duplicate Bearer prefix)

**Solutions**:
- Clear localStorage and log in again
- Check browser DevTools → Network → Request Headers
- Verify JWT Key in API appsettings.json

### 403 Forbidden

**Symptoms**: API returns 403 with "Direct access forbidden"

**Cause**: Missing or invalid `X-Gateway-Key` header

**Solutions**:
- Ensure Gateway is adding the header (check YARP transforms)
- Verify Gateway.ApiKey matches in both projects
- Restart Gateway after config changes

### Redirect Loop

**Symptoms**: User keeps getting redirected to login

**Cause**: Token not being stored or loaded correctly

**Solutions**:
- Check browser console for errors
- Verify `CustomAuthStateProvider` is registered
- Check localStorage for `authToken` key

## Best Practices

### ✅ Do

- Store JWT in `localStorage` (not cookies for Blazor WASM)
- Always validate tokens on the server
- Use HTTPS in production
- Rotate secrets regularly
- Implement token refresh for long sessions
- Log authentication failures

### ❌ Don't

- Store sensitive data in JWT (it's not encrypted, just encoded)
- Use short/weak JWT keys
- Skip token expiration validation
- Allow direct API access (always use Gateway)
- Hardcode secrets in code

## Future Enhancements

- [ ] Refresh tokens for longer sessions
- [ ] Role-based authorization (`[Authorize(Roles = "Admin")]`)
- [ ] Email confirmation requirement
- [ ] Two-factor authentication (2FA)
- [ ] OAuth providers (Google, Facebook)
- [ ] Account lockout after failed attempts
- [ ] Token revocation/blacklist

## Testing Authentication

### Manual Testing

1. Open browser DevTools (F12)
2. Go to Network tab
3. Login with credentials
4. Check Request Headers for subsequent requests
5. Should see: `Authorization: Bearer eyJ...`

### Test Accounts

After seeding, all users have password: `Pa$$w0rd`

```bash
dotnet run --project InstagramClone.Seeder -- --users 100 --posts 1
```

## Related Files

- **Client**:
  - `Services/CustomAuthStateProvider.cs`
  - `Services/AuthService.cs`
  - `Components/RedirectToLogin.razor`
  - `App.razor`
  - `Program.cs` (DI registration)

- **API**:
  - `Services/JwtTokenService.cs`
  - `Controllers/AuthController.cs`
  - `Middleware/GatewayAuthenticationMiddleware.cs`
  - `Program.cs` (JWT configuration)

- **Gateway**:
  - `Program.cs` (YARP transforms, CORS)
  - `appsettings.json` (Routes, CORS origins)

