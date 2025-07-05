using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class NurseSurgery
    {


        [ForeignKey("Nurse")]
        public string NurseId { get; set; }
        public Nurse? Nurse { get; set; }

        [ForeignKey("Surgery")]
        public string SurgeryId { get; set; }
        public Surgery? Surgery { get; set; }
    }
}
