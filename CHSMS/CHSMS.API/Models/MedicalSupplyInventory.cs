using System;
using System.Collections.Generic;

namespace CHSMS.API.Models
{
    public partial class MedicalSupplyInventory
    {
        public MedicalSupplyInventory()
        {
            MedicalSupplyConsumptions = new HashSet<MedicalSupplyConsumption>();
            MedicalSupplyInventoryStatistics = new HashSet<MedicalSupplyInventoryStatistic>();
        }

        public int SupplyInventoryId { get; set; }
        public int MedicalSupplyId { get; set; }
        public string? CertificateNumber { get; set; }
        public bool? TransactionType { get; set; }
        public double? Quantity { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ReceiverId { get; set; }
        public string? Note { get; set; }
        public string? BatchNumber { get; set; }
        public double? ImportQuantity { get; set; }

        public virtual MedicalSupply MedicalSupply { get; set; } = null!;
        public virtual User? Receiver { get; set; }
        public virtual ICollection<MedicalSupplyConsumption> MedicalSupplyConsumptions { get; set; }
        public virtual ICollection<MedicalSupplyInventoryStatistic> MedicalSupplyInventoryStatistics { get; set; }
    }
}
