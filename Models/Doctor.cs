using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Doctor
    {
        public string DoctorID { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Address { get; set; }
        public string DepartmentID { get; set; }

        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Surgery> Surgeries { get; set; }
        public ICollection<Description> Descriptions { get; set; }
    }

}
