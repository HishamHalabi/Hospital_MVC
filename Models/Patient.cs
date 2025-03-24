using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Patient
    {
        public string PatientID { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Prespection> Prespections { get; set; }
        public ICollection<Bill> Bills { get; set; }
       
    }

}
