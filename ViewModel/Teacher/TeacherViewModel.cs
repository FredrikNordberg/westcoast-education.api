using wescoast_education.api.ViewModels.Courses;

namespace westcoast_education.api.ViewModel
{
    public class TeacherViewModel
    {
        public Guid TeacherId { get; set; }
        public DateTime BirthOfDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public List<SkillViewModel> Skills { get; set; }
        public List<CourseViewModel> Courses { get; set; }
    }
}