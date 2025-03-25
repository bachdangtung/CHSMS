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
        public string? MedicineName { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Dosage { get; set; }
        public string? DosageForm { get; set; }
        public double? ImportPrice { get; set; }
        public double? SellingPrice { get; set; }
        public int? ShelfLife { get; set; }
        public string? BidNumber { get; set; }
        public bool? Status { get; set; }
        public bool? IsBhyt { get; set; }

        public virtual ICollection<MedicineInventory> MedicineInventories { get; set; }
    }
}
