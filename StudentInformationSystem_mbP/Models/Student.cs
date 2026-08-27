namespace StudentInformationSystem_mbP.Models;

public class Student
{
    public string Key { get; set; } = "";

    public string StudentId { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string FullName =>
        $"{FirstName} {LastName}";

    public string Email { get; set; } = "";

    public string Phone { get; set; } = "";

    public string Gender { get; set; } = "";

    public string BirthDate { get; set; } = "";

    public string Course { get; set; } = "";

    public string Address { get; set; } = "";

    public string ProfileImage { get; set; } = "";
}