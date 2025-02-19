using CHSMS.API.Models;

namespace CHSMS.API.DTOs.MedicalSupply
{
    public class MedicalSupplyDTO
    {
        //public MedicalSupplyDTO()
        //{
        //    SupplyConsumptionReports = new HashSet<SupplyConsumptionReport>();
        //    SupplyInventories = new HashSet<SupplyInventory>();
        //}

        public int MedicalSupplyId { get; set; }
        public string? Name { get; set; }
        public string? SupplyType { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int? SupplierId { get; set; }
        public string? Status { get; set; }
        public double? ImportPrice { get; set; }
        public double? SellingPrice { get; set; }
        public string? BatchNumber { get; set; }
        public int? BidNumber { get; set; }
        public virtual Supplier? Supplier { get; set; }
        //public virtual ICollection<SupplyConsumptionReport> SupplyConsumptionReports { get; set; }
        public virtual ICollection<SupplyInventoryDTO> SupplyInventories { get; set; }

        //new data
        public double? Quantity { get; set; }

    }
}
