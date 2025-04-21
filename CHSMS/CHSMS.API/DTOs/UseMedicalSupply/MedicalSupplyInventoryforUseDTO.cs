namespace CHSMS.API.DTOs.UseMedicalSupply
{
    public class MedicalSupplyInventoryforUseDTO
    {
        public int MedicalSupplyId { get; set; }
        public string MedicalSupplyName { get; set; }
        public int MedicalSupplyInventoryId { get; set; }
        public double Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        

    }
}
