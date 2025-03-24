using System.ComponentModel.DataAnnotations;

namespace Hospital.Models
{
    public class Room
    {
        [Key]
        public string RoomID { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string PatientID { get; set; }

        public Patient Patient { get; set; }
        public ICollection<NurseRoom> NurseRooms { get; set; }
    }

}
