using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Medicine
    {
        public Medicine()
        {
            MedicineInventories = new HashSet<MedicineInventory>();
        }

        public int MedicineId { get; set; }
        public string MedicineCode { get; set; } = null!;
        public string? MedicineName { get; set; }
        public string? TreatmentType { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Dosage { get; set; }
        public string? DosageForm { get; set; }
        public double? ImportPrice { get; set; }
        public double? SellingPrice { get; set; }
        public string? BatchNumber { get; set; }
        public string? BidNumber { get; set; }
        public int? Supplier { get; set; }

        public virtual Supplier? SupplierNavigation { get; set; }
        public virtual ICollection<MedicineInventory> MedicineInventories { get; set; }
    }
}
