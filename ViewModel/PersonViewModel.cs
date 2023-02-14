using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace westcoast_education.api.ViewModel
{
    public class PersonViewModel
    {
        
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