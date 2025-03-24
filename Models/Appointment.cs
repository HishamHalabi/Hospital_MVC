using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Appointment
    {
        public string AppointmentID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }

}
