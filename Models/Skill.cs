namespace westcoast_education.api.Models
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string SkillName { get; set; }

        // Navigation..
        // kompetens kan vara kopplad till flera lärare..
        public IList<Teacher> Teachers { get; set; }
    }
}