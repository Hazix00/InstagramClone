using System.Net.Http.Json;

namespace InstagramClone.Client.Services;

public class FollowService
{
    private readonly HttpClient _http;
    public FollowService(HttpClient http) => _http = http;

    public Task<HttpResponseMessage> FollowAsync(string username) =>
        _http.PostAsync($"api/follow/{username}", null);

    public Task<HttpResponseMessage> UnfollowAsync(string username) =>
        _http.DeleteAsync($"api/follow/{username}");

    public Task<FollowStatus?> GetStatusAsync(string username) =>
        _http.GetFromJsonAsync<FollowStatus>($"api/follow/status/{username}");
}

public class FollowStatus
{
    public bool IsFollowing { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
}


