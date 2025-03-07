using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class Medicine
    {
        public Medicine()
        {
            MedicalInventories = new HashSet<MedicalInventory>();
            MedicalUsages = new HashSet<MedicalUsage>();
        }

        public int MedicineId { get; set; }
        public string MedicineCode { get; set; } = null!;
        public string? MedicineName { get; set; }
        public string? TreatmentType { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Dosage { get; set; }
        public string? DosageForm { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? TotalAmount { get; set; }
        public string? BatchNumber { get; set; }
        public string? BidNumber { get; set; }

        public virtual ICollection<MedicalInventory> MedicalInventories { get; set; }
        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
    }
}
