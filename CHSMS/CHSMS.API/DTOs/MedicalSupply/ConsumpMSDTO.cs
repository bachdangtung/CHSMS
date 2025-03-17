namespace CHSMS.API.DTOs.MedicalSupply
{
    public class ConsumpMSDTO
    {
        public int MedicalSupplyId { get; set; }
        public double Quantity { get; set; }
        public bool? BHYT { get; set; }
        public string? Note { get; set; }
    }
}
