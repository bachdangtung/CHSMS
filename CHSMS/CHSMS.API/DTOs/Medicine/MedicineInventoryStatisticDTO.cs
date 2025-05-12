namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineInventoryStatisticDTO
    {
        public int MedicineInventoryStatisticsId { get; set; }
        public int MedicineInventoryId { get; set; }
        public double Quantity { get; set; }
        public double ActualQuantity { get; set; }
        public int StatisticPerson { get; set; }
        public int? ConfirmPerson { get; set; }
        public DateTime StatisticDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool IsUpdate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Note { get; set; }
    }
}
