using System.Net.Http.Json;
using InstagramClone.Core.DTOs;

namespace InstagramClone.Client.Services;

public class CommentService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private readonly string commentsEndpoint = "api/comments";

    public async Task<List<CommentDto>?> GetCommentsForPostAsync(Guid postId, int take = 20, int skip = 0)
    {
        var response = await _http.GetFromJsonAsync<CommentsResponse>($"{commentsEndpoint}/post/{postId}?take={take}&skip={skip}");
        return response?.Items;
    }

    public async Task<List<CommentDto>?> GetRepliesAsync(Guid parentCommentId, int take = 20, int skip = 0)
    {
        var response = await _http.GetFromJsonAsync<CommentsResponse>($"{commentsEndpoint}/replies/{parentCommentId}?take={take}&skip={skip}");
        return response?.Items;
    }

    public async Task<CommentDto?> AddCommentAsync(Guid postId, string content, Guid? parentCommentId = null)
    {
        var req = new CommentRequest { Content = content, ParentCommentId = parentCommentId };
        var resp = await _http.PostAsJsonAsync($"{commentsEndpoint}/post/{postId}", req);
        
        if (resp.IsSuccessStatusCode)
        {
            return await resp.Content.ReadFromJsonAsync<CommentDto>();
        }
        
        return null;
    }

    public async Task<bool> LikeAsync(Guid commentId)
    {
        var resp = await _http.PostAsync($"{commentsEndpoint}/{commentId}/like", null);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UnlikeAsync(Guid commentId)
    {
        var resp = await _http.DeleteAsync($"{commentsEndpoint}/{commentId}/like");
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid commentId)
    {
        var resp = await _http.DeleteAsync($"{commentsEndpoint}/{commentId}");
        return resp.IsSuccessStatusCode;
    }
}

public record CommentsResponse
{
    public int Total { get; init; }
    public List<CommentDto> Items { get; init; } = new();
}

