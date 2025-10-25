using System.Net.Http.Json;
using InstagramClone.Core.DTOs;

namespace InstagramClone.Client.Services;

public class PostService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private readonly string postsEndpoint = "api/posts";

    public Task<List<PostDto>?> GetMyPostsAsync() =>
        _http.GetFromJsonAsync<List<PostDto>>($"{postsEndpoint}/me");

    public Task<List<PostDto>?> GetFeedAsync(int take = 5, int skip = 0) =>
        _http.GetFromJsonAsync<List<PostDto>>($"{postsEndpoint}/feed?take={take}&skip={skip}");

    public Task<PostDto?> GetByIdAsync(Guid id) =>
        _http.GetFromJsonAsync<PostDto>($"{postsEndpoint}/{id}");

    public async Task<bool> CreateAsync(CreatePostRequest req)
    {
        var resp = await _http.PostAsJsonAsync(postsEndpoint, req);
        return resp.IsSuccessStatusCode;
    }

    public Task<HttpResponseMessage> DeleteAsync(Guid id) =>
        _http.DeleteAsync($"{postsEndpoint}/{id}");

    public async Task<bool> LikeAsync(Guid postId)
    {
        var resp = await _http.PostAsync($"{postsEndpoint}/{postId}/like", null);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UnlikeAsync(Guid postId)
    {
        var resp = await _http.DeleteAsync($"{postsEndpoint}/{postId}/like");
        return resp.IsSuccessStatusCode;
    }
}


