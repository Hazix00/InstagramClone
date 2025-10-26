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

    public static string Home_Welcome => Get(nameof(Home_Welcome), "Instagram");
    public static string Home_Tagline => Get(nameof(Home_Tagline), "Share moments with friends and family");
    public static string Home_JoinMessage => Get(nameof(Home_JoinMessage), "Join Instagram today to share photos, connect with friends, and explore amazing content from around the world.");
    public static string Home_AlreadyAccount => Get(nameof(Home_AlreadyAccount), "Already have an account?");
    public static string Home_BuiltWith => Get(nameof(Home_BuiltWith), "A modern Instagram clone built with Blazor WebAssembly");
    public static string Home_LoadingFeed => Get(nameof(Home_LoadingFeed), "Loading feed...");
    public static string Home_NoPosts => Get(nameof(Home_NoPosts), "No posts yet.");
    public static string Home_CreateOne => Get(nameof(Home_CreateOne), "Create one");
    public static string Home_LoadingMore => Get(nameof(Home_LoadingMore), "Loading more...");

    public static string PostCard_ViewAllComments => Get(nameof(PostCard_ViewAllComments), "View all");
    public static string PostCard_AddComment => Get(nameof(PostCard_AddComment), "Add a comment...");

    public static string CommentForm_AddComment => Get(nameof(CommentForm_AddComment), "Add a comment...");
    public static string CommentForm_ReplyTo => Get(nameof(CommentForm_ReplyTo), "Reply to {0}...");

    public static string CommentItem_ViewReplies => Get(nameof(CommentItem_ViewReplies), "View replies");

    public static string CommentsModal_Comments => Get(nameof(CommentsModal_Comments), "Comments");
    public static string CommentsModal_LoadingComments => Get(nameof(CommentsModal_LoadingComments), "Loading comments...");
    public static string CommentsModal_NoComments => Get(nameof(CommentsModal_NoComments), "No comments yet. Be the first to comment!");
    public static string CommentsModal_LoadingMore => Get(nameof(CommentsModal_LoadingMore), "Loading more comments...");

    public static string Theme_Toggle => Get(nameof(Theme_Toggle), "Toggle theme");

    public static string Common_Post => Get(nameof(Common_Post), "Post");
    public static string Common_Reply => Get(nameof(Common_Reply), "Reply");
    public static string Common_To => Get(nameof(Common_To), "to");
    public static string Common_Like => Get(nameof(Common_Like), "like");
    public static string Common_Likes => Get(nameof(Common_Likes), "likes");
    public static string Common_Comment => Get(nameof(Common_Comment), "comment");
    public static string Common_Comments => Get(nameof(Common_Comments), "comments");
}


