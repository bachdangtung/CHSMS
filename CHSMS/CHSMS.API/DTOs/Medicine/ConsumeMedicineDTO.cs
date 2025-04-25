namespace CHSMS.API.DTOs.Medicine
{
    public class ConsumeMedicineDTO
    {
        public int? ConsumeMedicineId { get; set; }
        public int? MedicineInventoryId { get; set; }
        public double? Quantity { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }
    }
}
