namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineStockDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public double? VirtualStock { get; set; }
        public double? ActualStock { get; set; }
    }
}
