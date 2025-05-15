namespace Hospital.Models
{
    public class Scan
    {
       
            public int ScanId { get; set; }
            public string Type { get; set; } // e.g., X-ray, MRI, CT, Ultrasound
            public DateTime ScanDate { get; set; }
            public string ResultDescription { get; set; }



           public string PatientId { get; set; }
           public Patient Patient { get; set; }
         
          public  string  DoctorId { get; set;  }
           public Doctor Doctor { get; set; }

           public string  PrescriptionId { get; set; }
           public Prescription Prescription { get; set; }
       

    }
}
