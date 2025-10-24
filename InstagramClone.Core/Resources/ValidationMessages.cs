using System.Resources;

namespace InstagramClone.Core.Resources;

public static class ValidationMessages
{
    private static readonly ResourceManager ResourceManager = new(
        "InstagramClone.Core.Resources.ValidationMessages",
        typeof(ValidationMessages).Assembly);

    private static string Get(string name, string fallback) => ResourceManager.GetString(name) ?? fallback;

    public static string Required_Username => Get(nameof(Required_Username), "Username is required.");
    public static string MinLength_Username => Get(nameof(MinLength_Username), "Username is too short.");
    public static string MaxLength_Username => Get(nameof(MaxLength_Username), "Username is too long.");
    public static string Invalid_UsernameFormat => Get(nameof(Invalid_UsernameFormat), "Username contains invalid characters.");

    public static string Required_Email => Get(nameof(Required_Email), "Email is required.");
    public static string Invalid_Email => Get(nameof(Invalid_Email), "Invalid email address.");
    public static string MaxLength_Email => Get(nameof(MaxLength_Email), "Email is too long.");

    public static string Required_Password => Get(nameof(Required_Password), "Password is required.");
    public static string MinLength_Password => Get(nameof(MinLength_Password), "Password is too short.");
    public static string MaxLength_Password => Get(nameof(MaxLength_Password), "Password is too long.");

    public static string Required_ConfirmPassword => Get(nameof(Required_ConfirmPassword), "Confirm password is required.");
    public static string Passwords_DoNotMatch => Get(nameof(Passwords_DoNotMatch), "Passwords do not match.");

    public static string Required_ImageUrl => Get(nameof(Required_ImageUrl), "Image URL is required.");
    public static string MaxLength_ImageUrl => Get(nameof(MaxLength_ImageUrl), "Image URL is too long.");
    public static string MaxLength_Caption => Get(nameof(MaxLength_Caption), "Caption is too long.");

    public static string Required_Content => Get(nameof(Required_Content), "Content is required.");
    public static string MaxLength_Content => Get(nameof(MaxLength_Content), "Content is too long.");

    public static string Required_Token => Get(nameof(Required_Token), "Token is required.");
}


