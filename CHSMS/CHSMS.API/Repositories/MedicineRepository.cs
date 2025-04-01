using CHSMS.API.DTOs.MedicineInventory;
using CHSMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class MedicineRepository
    {
        private readonly SEP_TestContext _context;
        public MedicineRepository(SEP_TestContext context)
        {
            _context = context;
        }
        public List<Medicine> GetAllMedicines()
        {
            return _context.Medicines.ToList();
        }

    }
}
