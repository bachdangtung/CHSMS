namespace CHSMS.API.DTOs.Medicine
{
    public class AddMedicineInventoryResultDTO
    {
        public bool IsSuccess { get; set; }
        public List<string> Warnings { get; set; } = new();
        public int AddedCount { get; set; }
    }
}
