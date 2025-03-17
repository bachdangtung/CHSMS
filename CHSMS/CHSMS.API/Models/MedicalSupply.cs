using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalSupply
    {
        public MedicalSupply()
        {
            MedicalSupplyInventories = new HashSet<MedicalSupplyInventory>();
        }

        public int MedicalSupplyId { get; set; }
        public string? MedicalSupplyName { get; set; }
        public string? SupplyType { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int? SupplierId { get; set; }
        public bool? Status { get; set; }
        public double? ImportPrice { get; set; }
        public double? SellingPrice { get; set; }
        public int? BidNumber { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<MedicalSupplyInventory> MedicalSupplyInventories { get; set; }
    }
}
