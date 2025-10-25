using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InstagramClone.Api.Attributes;

/// <summary>
/// Attribute to bind the current authenticated user to an action parameter.
/// Usage: public async Task<IActionResult> MyAction([CurrentUser] User user)
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class CurrentUserAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => BindingSource.Custom;
}

