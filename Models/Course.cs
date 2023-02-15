namespace westcoast_education.api.Models
{
    public class Course
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public int CourseNumber { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CourseStatusEnum Status { get; set; }

        // Navigering...
        // Aggregation...
        public IList<StudentCourse> StudentCourses { get; set; }

        // Composition
        public Guid? TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        
    }
}