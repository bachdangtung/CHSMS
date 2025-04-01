namespace CHSMS.API.DTOs.Prescription
{
    public class EditPrescriptionForPharmacistDTO
    {
        public int PrescriptionId { get; set; }
     
        public List<MedicineConsumptionStatusDTO> MedicineConsumptionStatuses { get; set; }
    }
    public class MedicineConsumptionStatusDTO
    {
        public int MedicineConsumptionId { get; set; }
        public bool Status { get; set; }
    }
}
