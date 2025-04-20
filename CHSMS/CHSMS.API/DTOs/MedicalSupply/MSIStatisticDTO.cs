namespace CHSMS.API.DTOs.MedicalSupply
{
    public class MSIStatisticDTO
    {
        public int? Msisid { get; set; }
        public int? MsinventoryId { get; set; }
        public double? Quantity { get; set; }
        public double? ActualQuantity { get; set; }
        public int? StatisticPerson { get; set; }
        public int? ConfirmPerson { get; set; }
        public DateTime? StatisticDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool? IsUpdate { get; set; }
        public string? Note { get; set; }
    }
}
