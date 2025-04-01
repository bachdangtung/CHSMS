namespace CHSMS.API.DTOs.MedicineInventory
{
    public class MedicineInventoryDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string ActiveIngredient { get; set; }
        public string Dosage { get; set; }
        public string DosageForm { get; set; }
        public int MedicineInventoryId { get; set; }
        public double Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } 
        public bool IsBhyt { get; set; }
    }
}
