namespace westcoast_education.api.Models
{
    public class Teacher : Person
    {
        //Navigation..
        //Aggregation..

        public IList<Course> Courses { get; set; }
        public IList<Skill> Skills { get; set; }
    }
}