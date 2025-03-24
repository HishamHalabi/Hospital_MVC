using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models
{
    public class Medicine
    {
        [Key]
        public string MedID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        public ICollection<PreMed> PreMeds { get; set; }
    }

}
