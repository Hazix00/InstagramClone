using InstagramClone.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace InstagramClone.Api.ModelBinders;

public class CurrentUserModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Check if the parameter has the [CurrentUser] attribute
        if (context.Metadata.ModelType == typeof(User) &&
            context.BindingInfo.BindingSource?.Id == "Custom")
        {
            // Return a BinderTypeModelBinder which will resolve dependencies from the request scope
            return new BinderTypeModelBinder(typeof(CurrentUserModelBinder));
        }

        return null;
    }
}

