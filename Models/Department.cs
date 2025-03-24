using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models
{
    public class Department
    {
        public string DepartmentID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Nurse> Nurses { get; set; }
    }

}
