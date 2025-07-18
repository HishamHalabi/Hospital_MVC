﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class PreMed
    {
        [ForeignKey("Medicine")]
        public string MedID { get; set; }

        [ForeignKey("Prescription")]
        public string PreID { get; set; }

        public string Dsoage { get; set; }
        public string Quantity { get; set; }

        public string NTimes { get; set; }

        public Medicine? Medicine { get; set; }
        public Prescription? Prescription { get; set; }
    }
}
