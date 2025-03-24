using System.ComponentModel.DataAnnotations;

namespace Hospital.Models
{
    public class Description

    {
        [Key]
        public string DescID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public bool Surgeries { get; set; }
        public string DescriptionText { get; set; }
        public DateTime DescDate { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }

}
