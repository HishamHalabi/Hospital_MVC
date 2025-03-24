namespace Hospital.Models
{
    public class NurseRoom
    {
        public string NurseID { get; set; }
        public string RoomID { get; set; }

        public Nurse Nurse { get; set; }
        public Room Room { get; set; }
    }

}
