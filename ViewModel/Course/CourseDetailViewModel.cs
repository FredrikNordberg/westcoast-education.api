using westcoast_education.api.Models;

namespace westcoast_education.api.ViewModel
{
    public class CourseDetailViewModel
    {
        public Guid CourseId { get; set ;}
        public string Title { get; set ; }
        public int CourseNumber { get; set; }
        public CourseStatusEnum Status { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TeacherDetailViewModel Teacher { get; set; } 
        public List<StudentDetailViewModel> Students { get; set; } 
    }
}