using System.Resources;

namespace InstagramClone.Client.Resources;

public static class UI
{
    private static readonly ResourceManager ResourceManager = new(
        "InstagramClone.Client.Resources.UI",
        typeof(UI).Assembly);

    private static string Get(string name, string fallback) => ResourceManager.GetString(name) ?? fallback;

    public static string Login_Title => Get(nameof(Login_Title), "Login • Instagram");
    public static string Login_UsernameOrEmail => Get(nameof(Login_UsernameOrEmail), "Username, or email");
    public static string Login_Password => Get(nameof(Login_Password), "Password");
    public static string Login_Submit => Get(nameof(Login_Submit), "Log in");
    public static string Login_Submitting => Get(nameof(Login_Submitting), "Logging in...");
    public static string Login_Forgot => Get(nameof(Login_Forgot), "Forgot password?");
    public static string Login_SignupPrompt => Get(nameof(Login_SignupPrompt), "Don't have an account?");

    public static string Common_Or => Get(nameof(Common_Or), "OR");
    public static string Common_Signup => Get(nameof(Common_Signup), "Sign up");
    public static string Common_Login => Get(nameof(Common_Login), "Log in");
    public static string Common_Email => Get(nameof(Common_Email), "Email");
    public static string Common_Sending => Get(nameof(Common_Sending), "Sending...");
    public static string Common_CreateAccount => Get(nameof(Common_CreateAccount), "Create new account");
    public static string Common_BackToLogin => Get(nameof(Common_BackToLogin), "Back to login");
    public static string Common_Submit => Get(nameof(Common_Submit), "Submit");
    public static string Common_Saving => Get(nameof(Common_Saving), "Saving...");
    public static string Common_Cancel => Get(nameof(Common_Cancel), "Cancel");
    public static string Common_Username => Get(nameof(Common_Username), "Username");
    public static string Common_Password => Get(nameof(Common_Password), "Password");
    public static string Common_EmailNotVerified => Get(nameof(Common_EmailNotVerified), "⚠ Your email is not verified");

    public static string Footer_Copyright => Get(nameof(Footer_Copyright), "© 2024 Instagram Clone");

    public static string Register_Title => Get(nameof(Register_Title), "Sign up • Instagram");
    public static string Register_Subtitle => Get(nameof(Register_Subtitle), "Sign up to see photos and videos from your friends.");
    public static string Register_ConfirmPassword => Get(nameof(Register_ConfirmPassword), "Confirm Password");
    public static string Register_Submitting => Get(nameof(Register_Submitting), "Signing up...");
    public static string Register_HaveAccount => Get(nameof(Register_HaveAccount), "Have an account?");

    public static string Forgot_Title => Get(nameof(Forgot_Title), "Reset Password • Instagram");
    public static string Forgot_Heading => Get(nameof(Forgot_Heading), "Trouble logging in?");
    public static string Forgot_Instruction => Get(nameof(Forgot_Instruction), "Enter your email and we'll send you a link to get back into your account.");
    public static string Forgot_SendLink => Get(nameof(Forgot_SendLink), "Send login link");

    public static string Verify_Title => Get(nameof(Verify_Title), "Verify Email • Instagram");
    public static string Verify_Heading => Get(nameof(Verify_Heading), "Verify your email");
    public static string Verify_Instruction => Get(nameof(Verify_Instruction), "Enter your email and verification code to verify your account.");
    public static string Verify_CodePlaceholder => Get(nameof(Verify_CodePlaceholder), "Verification code");
    public static string Verify_Submitting => Get(nameof(Verify_Submitting), "Verifying...");
    public static string Verify_Submit => Get(nameof(Verify_Submit), "Verify email");
    public static string Verify_Resend => Get(nameof(Verify_Resend), "Resend verification email");

    public static string Settings_Title => Get(nameof(Settings_Title), "Edit Profile • Instagram");
    public static string Settings_EditProfile => Get(nameof(Settings_EditProfile), "Edit Profile");
    public static string Settings_ChangePassword => Get(nameof(Settings_ChangePassword), "Change Password");
    public static string Settings_ChangePhoto => Get(nameof(Settings_ChangePhoto), "Change profile photo");
    public static string Settings_UsernameHint => Get(nameof(Settings_UsernameHint), "In most cases, you'll be able to change your username back for another 14 days.");

    public static string Error_NotAuthorized => Get(nameof(Error_NotAuthorized), "You are not authorized to access this resource.");
    public static string Error_NotFound => Get(nameof(Error_NotFound), "Not found");
    public static string Error_NotFoundMessage => Get(nameof(Error_NotFoundMessage), "Sorry, there's nothing at this address.");
}


