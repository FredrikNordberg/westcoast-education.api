namespace westcoast_education.api.ViewModel
{
    public class StudentPostViewModel : PersonViewModel
    {
        public IList<Guid> Courses { get; set; } = new List<Guid>();
    }
}