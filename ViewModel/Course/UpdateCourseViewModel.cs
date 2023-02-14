using System.ComponentModel.DataAnnotations;

namespace westcoast_education.api.ViewModel.Course
{
    public class UpdateCourseViewModel
    {
        
        [Required(ErrorMessage = "Kurstitel måste anges")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Kursnummer måste anges")]
        public int CourseNumber { get; set; }
        [Required(ErrorMessage = "Kursveckor måste anges")]
        public int Duration { get; set; }
        [Required(ErrorMessage = "Startdatum för kurs måste anges")]
        public DateTime StartDate { get; set; }
    }
}