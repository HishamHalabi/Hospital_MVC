using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Surgery
    {
        public string SurgeryID { get; set; }
        public string DoctorID { get; set; }
        public string PatientID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Type { get; set; }

        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }

}
