using System.Net.Http.Json;
using InstagramClone.Core.DTOs;

namespace InstagramClone.Client.Services;

public class PostService(HttpClient http)
{
    private readonly HttpClient _http = http;

    public Task<List<PostDto>?> GetMyPostsAsync() =>
        _http.GetFromJsonAsync<List<PostDto>>("api/posts/me");

    public Task<List<PostDto>?> GetFeedAsync(int take = 50, int skip = 0) =>
        _http.GetFromJsonAsync<List<PostDto>>($"api/posts/feed?take={take}&skip={skip}");

    public async Task<bool> CreateAsync(CreatePostRequest req)
    {
        var resp = await _http.PostAsJsonAsync("api/posts", req);
        return resp.IsSuccessStatusCode;
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id) =>
        _http.DeleteAsync($"api/posts/{id}");
}


