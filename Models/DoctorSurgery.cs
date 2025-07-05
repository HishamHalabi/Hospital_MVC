using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class DoctorSurgery
    {

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [ForeignKey("Surgery")]
        public string   SurgeryId { get; set; }
        public Surgery? Surgery { get; set; }


    }
}
