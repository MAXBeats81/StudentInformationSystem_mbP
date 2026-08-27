using StudentInformationSystem_mbP.Services;

namespace StudentInformationSystem_mbP.Pages;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseService _firebase;

    public LoginPage(FirebaseService firebase)
    {
        InitializeComponent();

        _firebase = firebase;
    }

    private async void LoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        // Check empty fields

        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ErrorLabel.Text =
                "Please enter your email and password.";

            ErrorLabel.IsVisible = true;

            return;
        }


        // Show loading

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        LoginButton.IsEnabled = false;


        // Firebase login

        bool success = await _firebase.LoginAsync(
            EmailEntry.Text.Trim(),
            PasswordEntry.Text);


        // Hide loading

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        LoginButton.IsEnabled = true;


        // Login failed

        if (!success)
        {
            ErrorLabel.Text =
                "Invalid email or password.";

            ErrorLabel.IsVisible = true;

            return;
        }


        // Login successful

        await Navigation.PushAsync(
            new DashboardPage(_firebase));
    }


    private void ShowPasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword =
            !PasswordEntry.IsPassword;
    }
}