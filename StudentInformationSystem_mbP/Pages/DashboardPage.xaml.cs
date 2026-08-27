using StudentInformationSystem_mbP.Services;

namespace StudentInformationSystem_mbP.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly FirebaseService _firebase;

    public DashboardPage(FirebaseService firebase)
    {
        InitializeComponent();

        _firebase = firebase;

        WelcomeLabel.Text = $"Welcome, {_firebase.CurrentUserEmail}";
        AccountLabel.Text = _firebase.CurrentUserEmail;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        int count = await _firebase.GetStudentCountAsync();

        TotalStudentsLabel.Text = count.ToString();

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private async void AddStudentClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new StudentFormPage(_firebase));
    }

    private async void ViewStudentsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new StudentListPage(_firebase));
    }

    private async void LogoutClicked(object sender, EventArgs e)
    {
        bool logout = await DisplayAlertAsync(
            "Logout",
            "Are you sure you want to log out?",
            "LOG OUT",
            "CANCEL");

        if (!logout)
            return;

        _firebase.Logout();

        await Navigation.PopToRootAsync();
    }
}