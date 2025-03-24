using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Nurse
    {
        public string NurseID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string DepartmentID { get; set; }

        public Department Department { get; set; }
     
        
        public ICollection<NurseRoom> NurseRooms { get; set; } = new List<NurseRoom>();
    }

}
