using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace westcoast_education.api.ViewModel
{
    public class TeacherPostViewModel : PersonViewModel
    {
        public IList<string> Skills { get; set; } = new List<string>();
    }
}