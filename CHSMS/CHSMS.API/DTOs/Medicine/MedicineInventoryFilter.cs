namespace CHSMS.API.DTOs.Medicine
{
    public class MedicineInventoryFilter
    {
        public bool ViewActualStock { get; set; }
        public bool ViewVirtualStock { get; set; }
        public double? MinimumStock { get; set; }
    }
}
