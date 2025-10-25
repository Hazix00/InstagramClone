using System.Security.Claims;
using InstagramClone.Api.Repositories;
using InstagramClone.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InstagramClone.Api.ModelBinders;

public class CurrentUserModelBinder : IModelBinder
{
    private readonly IUserRepository _userRepository;

    public CurrentUserModelBinder(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var user = httpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        // Try to get user by ID claim first
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user.FindFirst("sub")?.Value
                   ?? user.FindFirst("uid")?.Value;

        User? currentUser = null;

        if (int.TryParse(idClaim, out var userId))
        {
            currentUser = await _userRepository.GetByIdAsync(userId);
        }

        // Fallback to username
        if (currentUser == null)
        {
            var username = user.Identity?.Name
                        ?? user.FindFirst("preferred_username")?.Value
                        ?? user.FindFirst("unique_name")?.Value;

            if (!string.IsNullOrWhiteSpace(username))
            {
                currentUser = await _userRepository.GetByUsernameAsync(username);
            }
        }

        if (currentUser == null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        bindingContext.Result = ModelBindingResult.Success(currentUser);
    }
}

