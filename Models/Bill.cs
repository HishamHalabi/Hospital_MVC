using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Bill
    {
        public string BillID { get; set; }
        public string PatientID { get; set; }
        public int RoomCharges { get; set; }
        public int DoctorCharges { get; set; }
        public int MedicineCharges { get; set; }

        public Patient Patient { get; set; }
        public Payment Payment { get; set; }
    }

}
