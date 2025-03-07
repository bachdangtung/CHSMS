using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            MedicalSupplies = new HashSet<MedicalSupply>();
            Medicines = new HashSet<Medicine>();
        }

        public int SupplierId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ContactInfo { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<MedicalSupply> MedicalSupplies { get; set; }
        public virtual ICollection<Medicine> Medicines { get; set; }
    }
}
