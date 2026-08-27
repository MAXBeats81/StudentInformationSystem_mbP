using StudentInformationSystem_mbP.Models;
using StudentInformationSystem_mbP.Services;

namespace StudentInformationSystem_mbP.Pages;

public partial class StudentListPage : ContentPage
{
    private readonly FirebaseService _firebase;

    private List<Student> allStudents = new();

    public StudentListPage(FirebaseService firebase)
    {
        InitializeComponent();

        _firebase = firebase;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadStudents();
    }

    private async Task LoadStudents()
    {
        try
        {
            RefreshViewControl.IsRefreshing = true;

            allStudents = await _firebase.GetStudentsAsync();

            StudentCollectionView.ItemsSource = allStudents;
        }
        catch
        {
            await DisplayAlertAsync(
                "Error",
                "Unable to load students.",
                "OK");
        }
        finally
        {
            RefreshViewControl.IsRefreshing = false;
        }
    }

    private void SearchBar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        string search = e.NewTextValue?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(search))
        {
            StudentCollectionView.ItemsSource = allStudents;
            return;
        }

        var filtered = allStudents
            .Where(student =>
                student.StudentId.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

                || student.FirstName.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

                || student.LastName.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

                || student.Course.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        StudentCollectionView.ItemsSource = filtered;
    }

    private async void RefreshClicked(
        object sender,
        EventArgs e)
    {
        await LoadStudents();
    }

    private async void RefreshView_Refreshing(
        object sender,
        EventArgs e)
    {
        await LoadStudents();
    }

    private async void StudentSelected(
    object sender,
    SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault()
            is not Student student)
        {
            return;
        }

        StudentCollectionView.SelectedItem = null;

        await Navigation.PushAsync(
            new StudentFormPage(_firebase, student));
    }

    private async void DeleteClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not Student student)
        {
            return;
        }

        bool confirm = await DisplayAlertAsync(
            "Delete Student?",
            $"Are you sure you want to permanently delete {student.FullName}?",
            "DELETE",
            "CANCEL");

        if (!confirm)
            return;

        bool success =
            await _firebase.DeleteStudentAsync(student);

        if (!success)
        {
            await DisplayAlertAsync(
                "Error",
                "Unable to delete the student.",
                "OK");

            return;
        }

        await DisplayAlertAsync(
            "Deleted",
            "Student successfully deleted.",
            "OK");

        await LoadStudents();
    }
}