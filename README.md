# Instagram Clone with JWT Authentication

A full-stack Instagram clone application built with ASP.NET Core Web API and Blazor WebAssembly, featuring JWT authentication and Instagram-inspired UI.

## 🚀 Features

- ✅ JWT Bearer Authentication
- ✅ User Registration & Login
- ✅ Password Reset & Recovery
- ✅ Email Verification System
- ✅ Protected API Endpoints
- ✅ PostgreSQL Database with Entity Framework Core
- ✅ Swagger/OpenAPI Documentation
- ✅ Modern Blazor WebAssembly Client
- ✅ Tailwind CSS with Instagram-inspired UI
- ✅ Comprehensive Data Validation

## 🏗️ Architecture

### Backend (API)
- **Framework**: ASP.NET Core 9.0
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/Swashbuckle
- **Controllers**: RESTful API design

### Frontend (Client)
- **Framework**: Blazor WebAssembly
- **Authentication**: Custom AuthenticationStateProvider
- **Storage**: Blazored.LocalStorage for JWT tokens
- **UI**: Tailwind CSS with Instagram-inspired design
- **Styling**: Modern, responsive Instagram-like interface

## 📦 Project Structure

```
InstagramClone/
├── InstagramClone.Api/          # Backend API
│   ├── Controllers/
│   │   ├── AuthController.cs    # Login/Register endpoints
│   │   └── WeatherForecastController.cs
│   ├── Models/
│   │   ├── User.cs
│   │   ├── LoginRequest.cs
│   │   ├── RegisterRequest.cs
│   │   └── AuthResponse.cs
│   ├── Services/
│   │   └── JwtTokenService.cs   # JWT token generation
│   └── Program.cs
│
└── InstagramClone.Client/       # Frontend Blazor WASM
    ├── Pages/
    │   ├── Home.razor               # Welcome page
    │   ├── Login.razor              # Instagram-style login
    │   ├── Register.razor           # Instagram-style registration
    │   ├── ForgotPassword.razor     # Password recovery
    │   └── VerifyEmail.razor        # Email verification
    ├── Models/
    ├── Services/
    │   ├── AuthService.cs
    │   └── CustomAuthStateProvider.cs
    ├── wwwroot/
    │   ├── css/
    │   │   └── app.css              # Tailwind + Instagram styles
    │   └── index.html                # Tailwind CDN
    └── Program.cs
```

## 🔐 Authentication Flow

1. **User Registration**:
   - User submits registration form with username, email, and password
   - API creates new user account with hashed password
   - JWT token is generated and returned

2. **User Login**:
   - User submits credentials
   - API validates credentials
   - JWT token is generated and returned

3. **Token Storage**:
   - Client stores JWT token in browser's LocalStorage
   - Token is automatically attached to all HTTP requests

4. **Protected Routes**:
   - Client checks authentication state before rendering protected pages
   - API validates JWT token on protected endpoints

## 🛠️ Setup & Running

### Prerequisites
- .NET 9.0 SDK
- Your favorite code editor (Visual Studio, VS Code, or Rider)

### Configuration

1. **API Configuration** (`InstagramClone.Api/appsettings.json`):
```json
{
  "Jwt": {
    "Key": "YourSecretKeyHere-MustBeAtLeast32CharactersLong!",
    "Issuer": "InstagramCloneApi",
    "Audience": "InstagramCloneClient"
  }
}
```

⚠️ **Important**: Change the JWT Key in production!

2. **Client Configuration** (`InstagramClone.Client/Program.cs`):
```csharp
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7181") // Update to match your API URL
});
```

### Running the Application

1. **Start the API**:
```bash
cd InstagramClone.Api
dotnet run
```
The API will be available at `https://localhost:7181` (or your configured port).

2. **Start the Client** (in a new terminal):
```bash
cd InstagramClone.Client
dotnet run
```
The client will be available at `https://localhost:5001` (or your configured port).

### Testing with Swagger

1. Navigate to the API root URL (e.g., `https://localhost:7181`)
2. You'll see the Swagger UI
3. Test the authentication flow:
   - Register a new user via `/api/Auth/register`
   - Copy the returned token
   - Click "Authorize" button in Swagger
   - Enter: `Bearer {your-token-here}`
   - Now you can access protected endpoints like `/api/WeatherForecast`

## 🎯 Usage

### Register a New Account
1. Navigate to the Register page
2. Enter username, email, and password
3. Click "Register"
4. You'll be automatically logged in and redirected to the home page

### Login
1. Navigate to the Login page
2. Enter your username and password
3. Click "Login"
4. You'll be redirected to the home page

### Access Protected Content
- The Weather page demonstrates a protected route
- It fetches data from the protected API endpoint
- You must be logged in to access it

## 📝 API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with token
- `POST /api/auth/send-verification-email` - Send email verification
- `POST /api/auth/verify-email` - Verify email with token

## 🔒 Security Notes

- Passwords are hashed using SHA256 (use bcrypt or PBKDF2 in production)
- JWT tokens expire after 24 hours
- CORS is configured for localhost development
- In-memory user storage (replace with database in production)

## 🚧 Next Steps for Production

- [ ] Replace in-memory user storage with a database (e.g., Entity Framework Core)
- [ ] Use proper password hashing (bcrypt, Argon2, or PBKDF2)
- [ ] Implement refresh tokens
- [ ] Add email verification
- [ ] Implement password reset functionality
- [ ] Add rate limiting
- [ ] Configure proper CORS policies
- [ ] Use environment variables for sensitive configuration
- [ ] Add logging and monitoring
- [ ] Implement proper error handling

## 📚 Technologies Used

### Backend
- ASP.NET Core 9.0
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore (Swagger)
- System.IdentityModel.Tokens.Jwt

### Frontend
- Blazor WebAssembly 9.0
- Blazored.LocalStorage
- Microsoft.AspNetCore.Components.Authorization
- Tailwind CSS (via CDN)
- Inter & Billabong fonts (Google Fonts)

## 📄 License

This is a sample project for educational purposes.

