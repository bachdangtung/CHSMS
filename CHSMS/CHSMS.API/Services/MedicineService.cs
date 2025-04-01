using CHSMS.API.DTOs.MedicineInventory;
using CHSMS.API.Repositories;

namespace CHSMS.API.Services
{
    public class MedicineService
    {
        private readonly MedicineRepository _medicineRepository;

        public MedicineService(MedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        
    }
}
