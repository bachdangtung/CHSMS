using CHSMS.API.Models;
using CHSMS.API.DTOs;

namespace CHSMS.API.DTOs.MedicalSupply
{
    public class MedicalSupplyDTO
    {
        public MedicalSupplyDTO()
        {
            MedicalSupplyInventories = new HashSet<MedicalSupplyInventoryDTO>();
        }

        public int MedicalSupplyId { get; set; }
        public string? MedicalSupplyName { get; set; }
        public string? SupplyType { get; set; }
        public string? UnitOfMeasure { get; set; }
        public int? SupplierId { get; set; }
        public bool? Status { get; set; }
        public double? ImportPrice { get; set; }
        public double? SellingPrice { get; set; }
        public string? BatchNumber { get; set; }
        public int? BidNumber { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<MedicalSupplyInventoryDTO> MedicalSupplyInventories { get; set; }


        public double? Quantity { get; set; }
    }
}
