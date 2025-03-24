using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Payment
    {
        [Key]
        public string PaymentID { get; set; }
        public string BillID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public Bill Bill { get; set; }
    }

}
