namespace westcoast_education.api.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public DateTime BirthOfDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
    }
}