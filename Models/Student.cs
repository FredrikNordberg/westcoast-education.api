namespace westcoast_education.api.Models
{
    public class Student : Person
    {
        // Navigering..

       public IList<StudentCourse> StudentCourses { get; set; }
    }

}