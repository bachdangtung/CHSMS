using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Medicine
    {
        public Medicine()
        {
            MedicineConsumptions = new HashSet<MedicineConsumption>();
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

        public virtual ICollection<MedicineConsumption> MedicineConsumptions { get; set; }
        public virtual ICollection<MedicineInventory> MedicineInventories { get; set; }
    }
}
