namespace westcoast_education.api.ViewModel.Student
{
    public class StudentListViewModel
    {
        public Guid StudentId { get; set; }
        public DateTime BirthOfDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        
        public List<CourseListViewModel> Courses { get; set; }
    }
}