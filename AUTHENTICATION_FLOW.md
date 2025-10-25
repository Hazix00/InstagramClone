# Global Authentication Flow

## Overview

This application implements **two-layer authentication validation**:

1. **Client-side JWT validation** - Checks token expiration when visiting pages
2. **API response validation** - Handles 401 errors from API calls globally

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│  User navigates to protected page                       │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│  CustomAuthStateProvider.GetAuthenticationStateAsync()  │
│  • Reads token from localStorage                        │
│  • Checks token expiration (IsTokenExpired)             │
│  • If expired: Clear token, return unauthenticated      │
│  • If valid: Return authenticated state                 │
└────────────────┬────────────────────────────────────────┘
                 │
                 ├─────► Expired? → Redirect to login (by AuthorizeRouteView)
                 │
                 └─────► Valid? → Continue
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│  User makes API call (e.g., fetch posts)                │
└────────────────┬────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────┐
│  AuthenticationHandler.SendAsync()                      │
│  • Intercepts HTTP request                              │
│  • Adds Authorization: Bearer {token} header            │
│  • Sends request to API                                 │
│  • Checks response status                               │
│  • If 401: Clear token, notify logout, redirect         │
└────────────────┬────────────────────────────────────────┘
                 │
                 ├─────► 401? → Clear localStorage → Redirect to login
                 │
                 └─────► Success? → Return response
```

## Implementation Details

### 1. Token Validation on Page Visit

**File**: `InstagramClone.Client/Services/CustomAuthStateProvider.cs`

```csharp
public override async Task<AuthenticationState> GetAuthenticationStateAsync()
{
    var token = await _localStorage.GetItemAsStringAsync(TokenKey);

    if (string.IsNullOrEmpty(token))
    {
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    token = token.Trim('"');

    // ✅ CHECK 1: Validate token expiration before using it
    if (IsTokenExpired(token))
    {
        // Token is expired, clear it and return unauthenticated state
        await _localStorage.RemoveItemAsync(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    // Token is valid, set up authenticated state
    _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);

    var claims = ParseClaimsFromJwt(token);
    var identity = new ClaimsIdentity(claims, "jwt");
    var user = new ClaimsPrincipal(identity);

    return new AuthenticationState(user);
}

private static bool IsTokenExpired(string jwt)
{
    try
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        
        // Check if token has expired (exp claim is in Unix timestamp)
        var expClaim = token.Claims.FirstOrDefault(c => c.Type == "exp");
        if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            return DateTime.UtcNow >= expirationDate;
        }
        
        return false; // If no exp claim, assume not expired
    }
    catch
    {
        return true; // If token is malformed, consider it expired
    }
}
```

**What happens**:
- User navigates to `/profile` (protected page)
- `AuthorizeRouteView` in `App.razor` checks authentication state
- `CustomAuthStateProvider` reads token from `localStorage`
- Token expiration is validated (checks `exp` claim)
- If expired: Token is removed, user sees login page
- If valid: User sees the protected page

### 2. Global 401 Handling on API Calls

**File**: `InstagramClone.Client/Services/AuthenticationHandler.cs`

```csharp
public class AuthenticationHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get token from localStorage
        var token = await _localStorage.GetItemAsStringAsync(TokenKey);

        if (!string.IsNullOrEmpty(token))
        {
            token = token.Trim('"');
            
            // Add Authorization header if not already present
            if (request.Headers.Authorization == null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // ✅ CHECK 2: Handle 401 Unauthorized globally
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Token is invalid or expired (validated by API)
            await _localStorage.RemoveItemAsync(TokenKey);
            
            // Notify auth state provider of logout
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.NotifyUserLogout();
            }

            // Redirect to login
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}
```

**What happens**:
- User is on `/profile` page and loads their posts
- `PostService.GetMyPostsAsync()` calls the API
- `AuthenticationHandler` intercepts the request
- Adds `Authorization: Bearer {token}` header
- Sends request to API via Gateway
- API validates token and returns 401 (e.g., signature invalid, expired)
- `AuthenticationHandler` catches 401 response
- Token is removed from `localStorage`
- Auth state is updated (notifies all components)
- User is redirected to `/login` with `forceLoad: true`

### 3. Registration in Program.cs

**File**: `InstagramClone.Client/Program.cs`

```csharp
// Register the authentication handler
builder.Services.AddScoped<AuthenticationHandler>();

// Configure HttpClient with Gateway base address and authentication handler
var gatewayUrl = builder.Configuration["ApiGateway:BaseUrl"] 
                 ?? throw new InvalidOperationException("ApiGateway:BaseUrl is not configured");

builder.Services.AddHttpClient("API", client => 
{
    client.BaseAddress = new Uri(gatewayUrl);
})
.AddHttpMessageHandler<AuthenticationHandler>(); // ✅ Global interceptor

// Register the default HttpClient (uses the named "API" client)
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));
```

## Why Two Layers?

### Layer 1: Client-Side Validation (Performance)
- **Fast**: No API call needed
- **Proactive**: Catches expired tokens before making requests
- **User Experience**: Immediate redirect, no failed API calls

### Layer 2: API Response Validation (Security)
- **Secure**: API is the source of truth
- **Catches edge cases**: Signature tampering, revoked tokens, clock skew
- **Automatic cleanup**: Works for any API call, anywhere in the app

## Complete Flow Example

### Scenario 1: Token Expired on Page Visit

```
1. User navigates to /profile
   ↓
2. AuthorizeRouteView calls GetAuthenticationStateAsync()
   ↓
3. CustomAuthStateProvider reads token from localStorage
   ↓
4. IsTokenExpired(token) → TRUE (exp: 1761420583, now: 1761420600)
   ↓
5. Token removed from localStorage
   ↓
6. Returns unauthenticated state
   ↓
7. AuthorizeRouteView shows <RedirectToLogin />
   ↓
8. User redirected to /login
```

### Scenario 2: Token Valid but API Returns 401

```
1. User on /profile page (token was valid on navigation)
   ↓
2. User clicks "View Posts" → API call
   ↓
3. AuthenticationHandler intercepts request
   ↓
4. Adds Authorization: Bearer {token} header
   ↓
5. Gateway forwards to API with X-Gateway-Key
   ↓
6. API validates JWT → FAILS (e.g., signature tampered)
   ↓
7. API returns 401 Unauthorized
   ↓
8. AuthenticationHandler catches 401
   ↓
9. Token removed from localStorage
   ↓
10. NotifyUserLogout() updates auth state
   ↓
11. NavigateTo("/login", forceLoad: true)
   ↓
12. User redirected to /login
```

### Scenario 3: Token Valid

```
1. User navigates to /profile
   ↓
2. CustomAuthStateProvider validates token → VALID
   ↓
3. Returns authenticated state
   ↓
4. AuthorizeRouteView shows profile page
   ↓
5. User clicks "View Posts" → API call
   ↓
6. AuthenticationHandler adds token header
   ↓
7. API validates JWT → SUCCESS
   ↓
8. API returns 200 OK with posts
   ↓
9. AuthenticationHandler returns response
   ↓
10. Posts displayed on page
```

## Benefits

### ✅ No Code Duplication
- Authentication logic is centralized in two global services
- No need to check auth state in every page
- No need to handle 401 in every API call

### ✅ Automatic Cleanup
- Token removed from `localStorage` automatically
- Auth state updated globally
- All components notified of logout

### ✅ Consistent UX
- User always redirected to `/login` when unauthenticated
- No confusing error messages
- `forceLoad: true` ensures clean state

### ✅ Performance
- Client-side validation prevents unnecessary API calls
- API validation ensures security

### ✅ Maintainable
- All authentication logic in two files:
  - `CustomAuthStateProvider.cs` (token validation)
  - `AuthenticationHandler.cs` (401 handling)

## Testing

### Test Case 1: Expired Token on Page Visit

1. Login to the app
2. Open browser DevTools → Application → LocalStorage
3. Get the `authToken` value
4. Decode at jwt.io and note the `exp` claim
5. Wait for token to expire (or manually set device time forward)
6. Navigate to `/profile`
7. **Expected**: Immediate redirect to `/login`

### Test Case 2: Invalid Token on API Call

1. Login to the app
2. Navigate to `/profile` (should work)
3. Open DevTools → Application → LocalStorage
4. Modify the `authToken` value (change any character)
5. Refresh the page
6. **Expected**: Token validation passes (no API call yet), but first API call returns 401
7. **Expected**: Automatic redirect to `/login`

### Test Case 3: Direct 401 from API

1. Stop the API but keep Gateway/Client running
2. Login to the app (will fail, but you can manually set a fake token)
3. Try to access any protected resource
4. **Expected**: 401 from API → Redirect to `/login`

## Future Enhancements

- [ ] Add token refresh before expiration
- [ ] Show a countdown "Session expires in X minutes"
- [ ] Offer "Stay logged in" option with longer expiration
- [ ] Add retry logic for transient 401 errors
- [ ] Implement token blacklist on server side
- [ ] Add activity tracking (extend session on user activity)

## Related Files

- `InstagramClone.Client/Services/CustomAuthStateProvider.cs` - Token validation on page visit
- `InstagramClone.Client/Services/AuthenticationHandler.cs` - 401 handling on API calls
- `InstagramClone.Client/Program.cs` - HttpClient registration
- `InstagramClone.Client/App.razor` - AuthorizeRouteView configuration
- `InstagramClone.Client/Components/RedirectToLogin.razor` - Login redirect component

