using System.Security.Claims;
using InstagramClone.Api.Repositories;
using InstagramClone.Api.Services;
using InstagramClone.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstagramClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;
    private readonly IUserRepository _userRepository;

    public FollowController(IFollowService followService, IUserRepository userRepository)
    {
        _followService = followService;
        _userRepository = userRepository;
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? User.FindFirst("uid")?.Value;

        if (int.TryParse(idClaim, out var id))
            return await _userRepository.GetByIdAsync(id);

        var username = User.Identity?.Name
                    ?? User.FindFirst("preferred_username")?.Value
                    ?? User.FindFirst("unique_name")?.Value;

        if (string.IsNullOrWhiteSpace(username)) return null;
        return await _userRepository.GetByUsernameAsync(username);
    }

    [HttpPost("{username}")]
    public async Task<IActionResult> Follow(string username)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var target = await _userRepository.GetByUsernameAsync(username);
        if (target is null) return NotFound("User not found.");
        if (target.Id == me.Id) return BadRequest("Cannot follow yourself.");

        await _followService.FollowUserAsync(me.Id, target.Id);
        return NoContent();
    }

    [HttpDelete("{username}")]
    public async Task<IActionResult> Unfollow(string username)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var target = await _userRepository.GetByUsernameAsync(username);
        if (target is null) return NotFound("User not found.");

        await _followService.UnfollowUserAsync(me.Id, target.Id);
        return NoContent();
    }

    [HttpGet("status/{username}")]
    public async Task<ActionResult<object>> Status(string username)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var target = await _userRepository.GetByUsernameAsync(username);
        if (target is null) return NotFound("User not found.");

        var isFollowing = await _followService.IsFollowingAsync(me.Id, target.Id);
        var followers = await _followService.GetFollowerCountAsync(target.Id);
        var following = await _followService.GetFollowingCountAsync(target.Id);

        return Ok(new { isFollowing, followers, following });
    }
}
