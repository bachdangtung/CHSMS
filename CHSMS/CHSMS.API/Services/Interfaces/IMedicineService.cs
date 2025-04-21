using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;

namespace CHSMS.API.Services.Interfaces
{
    public interface IMedicineService
    {
        List<MedicineDTO> GetAll();
        MedicineDTO? GetMedicineById(int medicineId);
        List<MedicineInventoryDetailDTO> MedicineDetail(int medicineId);
        bool AddMedicineInventory(MedicineInventoryAddDTO medicineInventory);
        bool UpdateMedicineInventory(MedicineInventoryDetailDTO medicineInventory);
    }
}
