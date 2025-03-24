using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Prespection
    {
        [Key]
        public string PrescriptionID { get; set; }
        public string PatientID { get; set; }
        public string DescID { get; set; }
        public string Dosage { get; set; }

        public Patient Patient { get; set; }
        public Description Description { get; set; }
        public ICollection<PreMed> PreMeds { get; set; }
    }

}
