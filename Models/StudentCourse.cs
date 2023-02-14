namespace westcoast_education.api.Models
{
    public class StudentCourse
    {
        // Kopplingstabell..

        public CourseStatusEnum Status { get; set; }

        // Navigation..
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; }
    }
}