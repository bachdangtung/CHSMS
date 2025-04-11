namespace CHSMS.API.DTOs.MedicalSupply
{
    public class ConsumpMSDTO
    {
        public int? ConsumpMSID { get; set; }
        public int? MedicalSupplyInventoryId { get; set; }
        public double? Quantity { get; set; }
        public string? Note { get; set; }
    }
}
