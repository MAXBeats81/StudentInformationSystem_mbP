using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using StudentInformationSystem_mbP.Models;

namespace StudentInformationSystem_mbP.Services;

public class FirebaseService
{
    private const string ApiKey =
        "AIzaSyAIeUCPvENtNW8XHfsE3NNiGjqKoxwaRK4";

    private const string DatabaseUrl =
        "https://studentinformationsystem-mbp-default-rtdb.asia-southeast1.firebasedatabase.app";

    private readonly FirebaseAuthClient _authClient;
    private readonly FirebaseClient _firebaseClient;

    public FirebaseService()
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = ApiKey,
            AuthDomain = "studentinformationsystem-mbp.firebaseapp.com",
            Providers =
            [
                new EmailProvider()
            ]
        };

        _authClient = new FirebaseAuthClient(config);

        _firebaseClient = new FirebaseClient(
    DatabaseUrl,
    new FirebaseOptions
    {
        AuthTokenAsyncFactory = async () =>
        {
            if (_authClient.User == null)
                return null;

            return await _authClient.User.GetIdTokenAsync();
        }
    });
    }


    // =========================
    // AUTHENTICATION
    // =========================

    public async Task<bool> LoginAsync(
        string email,
        string password)
    {
        try
        {
            await _authClient
                .SignInWithEmailAndPasswordAsync(
                    email,
                    password);

            return true;
        }
        catch
        {
            return false;
        }
    }


    public void Logout()
    {
        _authClient.SignOut();
    }


    // IMPORTANT:
    // DashboardPage uses CurrentUserEmail

    public string CurrentUserEmail
    {
        get
        {
            return _authClient.User?.Info?.Email
                   ?? "";
        }
    }


    // =========================
    // CREATE
    // =========================

    public async Task<bool> AddStudentAsync(
        Student student)
    {
        try
        {
            await _firebaseClient
                .Child("Students")
                .PostAsync(student);

            return true;
        }
        catch
        {
            return false;
        }
    }


    // =========================
    // READ
    // =========================

    public async Task<List<Student>> GetStudentsAsync()
    {
        try
        {
            var students = await _firebaseClient
                .Child("Students")
                .OnceAsync<Student>();

            return students
                .Select(item =>
                {
                    item.Object.Key = item.Key;

                    return item.Object;
                })
                .ToList();
        }
        catch
        {
            return new List<Student>();
        }
    }


    // =========================
    // STUDENT COUNT
    // =========================

    public async Task<int> GetStudentCountAsync()
    {
        try
        {
            var students = await _firebaseClient
                .Child("Students")
                .OnceAsync<Student>();

            return students.Count;
        }
        catch
        {
            return 0;
        }
    }


    // =========================
    // UPDATE
    // =========================

    public async Task<bool> UpdateStudentAsync(
        Student student)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(student.Key))
                return false;

            await _firebaseClient
                .Child("Students")
                .Child(student.Key)
                .PutAsync(student);

            return true;
        }
        catch
        {
            return false;
        }
    }

    // =========================
    // DELETE
    // =========================

    public async Task<bool> DeleteStudentAsync(
        Student student)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(student.Key))
                return false;

            await _firebaseClient
                .Child("Students")
                .Child(student.Key)
                .DeleteAsync();

            return true;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Firebase Error",
                ex.Message,
                "OK");

            return false;
        }
    }
}