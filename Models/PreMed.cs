using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class PreMed
    { 

        public string MedID { get; set; }
        public string PreID { get; set; }

        public Medicine Medicine { get; set; }
        public Prespection Prespection { get; set; }
    }
}
