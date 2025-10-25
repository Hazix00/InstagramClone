using InstagramClone.Api.Attributes;
using InstagramClone.Api.Repositories;
using InstagramClone.Api.Services;
using InstagramClone.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstagramClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowController(IFollowService followService, IUserRepository userRepository) 
    : ControllerBase
{
    private readonly IFollowService _followService = followService;
    private readonly IUserRepository _userRepository = userRepository;

    [HttpPost("{username}")]
    public async Task<IActionResult> Follow([CurrentUser] User me, string username)
    {

        var target = await _userRepository.GetByUsernameAsync(username);
        if (target is null) return NotFound("User not found.");
        if (target.Id == me.Id) return BadRequest("Cannot follow yourself.");

        await _followService.FollowUserAsync(me.Id, target.Id);
        return NoContent();
    }

    [HttpDelete("{username}")]
    public async Task<IActionResult> Unfollow([CurrentUser] User me, string username)
    {

        var target = await _userRepository.GetByUsernameAsync(username);
        if (target is null) return NotFound("User not found.");

        await _followService.UnfollowUserAsync(me.Id, target.Id);
        return NoContent();
    }

    [HttpGet("status/{username}")]
    public async Task<ActionResult<object>> Status([CurrentUser] User me, string username)
    {

        var target = await _userRepository.GetByUsernameAsync(username);
        if (target is null) return NotFound("User not found.");

        var isFollowing = await _followService.IsFollowingAsync(me.Id, target.Id);
        var followers = await _followService.GetFollowerCountAsync(target.Id);
        var following = await _followService.GetFollowingCountAsync(target.Id);

        return Ok(new { isFollowing, followers, following });
    }
}
