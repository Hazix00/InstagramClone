using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using InstagramClone.Core.DTOs;

namespace InstagramClone.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    private const string UserKey = "authUser";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    await _localStorage.SetItemAsStringAsync(TokenKey, authResponse.Token);
                    await _localStorage.SetItemAsync(UserKey, new { authResponse.Username, authResponse.Email });
                    return authResponse;
                }
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    await _localStorage.SetItemAsStringAsync(TokenKey, authResponse.Token);
                    await _localStorage.SetItemAsync(UserKey, new { authResponse.Username, authResponse.Email });
                    return authResponse;
                }
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsStringAsync(TokenKey);
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}

