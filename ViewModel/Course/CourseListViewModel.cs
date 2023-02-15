using westcoast_education.api.Models;

namespace westcoast_education.api.ViewModel
{
    public class CourseListViewModel
    {
        public Guid CourseId { get; set ;}
        public string Title { get; set ; }
        public int CourseNumber { get; set; }
        public int Duration { get; set; }
        public CourseStatusEnum Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        
        public List<StudentDetailViewModel> Students { get; set; }
        public List<TeacherDetailViewModel> Teachers { get; set; }
        
    }
}