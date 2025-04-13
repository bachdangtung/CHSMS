namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineDTO
    {
        public int MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Dosage { get; set; }
        public string? DosageForm { get; set; }
        public double? Quantity { get; set; }
        public double? ImportPrice { get; set; }
        public double? SellingPrice { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ShelfLife { get; set; }
        public string? BatchNumber { get; set; }
        public string? BidNumber { get; set; }
        public bool? Status { get; set; }
        public bool? IsBhyt { get; set; }
    }
}
