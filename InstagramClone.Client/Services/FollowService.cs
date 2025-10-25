using System.Net.Http.Json;

namespace InstagramClone.Client.Services;

public class FollowService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private readonly string followEndpoint = "api/follow";

    public Task<HttpResponseMessage> FollowAsync(string username) =>
        _http.PostAsync($"{followEndpoint}/{username}", null);

    public Task<HttpResponseMessage> UnfollowAsync(string username) =>
        _http.DeleteAsync($"{followEndpoint}/{username}");

    public Task<FollowStatus?> GetStatusAsync(string username) =>
        _http.GetFromJsonAsync<FollowStatus>($"{followEndpoint}/status/{username}");
}

public class FollowStatus
{
    public bool IsFollowing { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
}


