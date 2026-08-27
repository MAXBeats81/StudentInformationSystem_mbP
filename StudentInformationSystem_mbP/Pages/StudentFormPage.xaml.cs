using System.Net.Mail;
using StudentInformationSystem_mbP.Models;
using StudentInformationSystem_mbP.Services;

namespace StudentInformationSystem_mbP.Pages;

public partial class StudentFormPage : ContentPage
{
    private readonly FirebaseService _firebase;

    private string selectedImage = "";

    private Student? editingStudent;

    public StudentFormPage(FirebaseService firebase)
    {
        InitializeComponent();

        _firebase = firebase;

        Title = "Add Student";
    }

    public StudentFormPage(
        FirebaseService firebase,
        Student student)
    {
        InitializeComponent();

        _firebase = firebase;

        editingStudent = student;

        Title = "Edit Student";

        LoadStudentData(student);
    }

    private void LoadStudentData(Student student)
    {
        StudentIdEntry.Text = student.StudentId;
        FirstNameEntry.Text = student.FirstName;
        LastNameEntry.Text = student.LastName;
        EmailEntry.Text = student.Email;
        PhoneEntry.Text = student.Phone;
        CourseEntry.Text = student.Course;
        AddressEditor.Text = student.Address;

        if (student.Gender == "Male")
            GenderPicker.SelectedIndex = 0;
        else if (student.Gender == "Female")
            GenderPicker.SelectedIndex = 1;
        else if (student.Gender == "Other")
            GenderPicker.SelectedIndex = 2;

        if (DateTime.TryParse(
            student.BirthDate,
            out DateTime birthDate))
        {
            BirthDatePicker.Date = birthDate;
        }

        selectedImage = student.ProfileImage;

        if (!string.IsNullOrWhiteSpace(selectedImage))
        {
            ProfileImageView.Source =
                ImageSource.FromFile(selectedImage);
        }

        SaveButton.Text = "UPDATE STUDENT";
    }

    private async void SelectImageClicked(
        object sender,
        EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Select a profile image"
                });

            if (result == null)
                return;

            selectedImage = result.FullPath;

            ProfileImageView.Source =
                ImageSource.FromFile(selectedImage);
        }
        catch
        {
            await DisplayAlertAsync(
                "Image Error",
                "Unable to select the profile image.",
                "OK");
        }
    }

    private async void SaveStudentClicked(
        object sender,
        EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(StudentIdEntry.Text))
        {
            ShowError("Student ID is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
        {
            ShowError("First name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
        {
            ShowError("Last name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            ShowError("Email is required.");
            return;
        }

        if (!IsValidEmail(EmailEntry.Text))
        {
            ShowError("Invalid email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            ShowError("Phone number is required.");
            return;
        }

        if (GenderPicker.SelectedIndex == -1)
        {
            ShowError("Please select a gender.");
            return;
        }

        if (string.IsNullOrWhiteSpace(CourseEntry.Text))
        {
            ShowError("Course is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(AddressEditor.Text))
        {
            ShowError("Address is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedImage))
        {
            ShowError("Please select a profile image.");
            return;
        }

        var student = new Student
        {
            Key = editingStudent?.Key ?? "",

            StudentId = StudentIdEntry.Text.Trim(),

            FirstName = FirstNameEntry.Text.Trim(),

            LastName = LastNameEntry.Text.Trim(),

            Email = EmailEntry.Text.Trim(),

            Phone = PhoneEntry.Text.Trim(),

            Gender = GenderPicker.SelectedItem?.ToString() ?? "",

            BirthDate = BirthDatePicker.Date.ToString(),

            Course = CourseEntry.Text.Trim(),

            Address = AddressEditor.Text.Trim(),

            ProfileImage = selectedImage
        };

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        SaveButton.IsEnabled = false;

        bool success;

        if (editingStudent == null)
        {
            success =
                await _firebase.AddStudentAsync(student);
        }
        else
        {
            success =
                await _firebase.UpdateStudentAsync(student);
        }

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        SaveButton.IsEnabled = true;

        if (!success)
        {
            ShowError(
                editingStudent == null
                    ? "Unable to save student."
                    : "Unable to update student.");

            return;
        }

        await DisplayAlertAsync(
            "Success",
            editingStudent == null
                ? "Student successfully added!"
                : "Student successfully updated!",
            "OK");

        await Navigation.PopAsync();
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);

            return address.Address == email;
        }
        catch
        {
            return false;
        }
    }
}