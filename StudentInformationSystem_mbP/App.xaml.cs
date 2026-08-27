using StudentInformationSystem_mbP.Services;
using StudentInformationSystem_mbP.Pages;

namespace StudentInformationSystem_mbP;

public partial class App : Application
{
    public App(FirebaseService firebase)
    {
        InitializeComponent();

        MainPage = new NavigationPage(
            new LoginPage(firebase));
    }
}