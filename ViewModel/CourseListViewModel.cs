namespace westcoast_education.api.ViewModel
{
    public class CourseListViewModel
    {
        public Guid CourseId { get; set ;}
        public string Title { get; set ; }
        public int CourseNumber { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}