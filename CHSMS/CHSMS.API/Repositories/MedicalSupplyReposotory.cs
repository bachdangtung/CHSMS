using CHSMS.API.Models;
namespace CHSMS.API.Repositories
{
    public class MedicalSupplyReposotory
    {
        private readonly SEP_TestContext _context;
        public MedicalSupplyReposotory(SEP_TestContext context)
        {
            _context = context;
        }
        public List<MedicalSupply> GetAllMedicalSupplies()
        {
            return _context.MedicalSupplies.ToList();
        }
        public double? GetSupplyQuantity(int medicalSupplyId)
        {
            return _context.MedicalSupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId && x.ExpiryDate > DateTime.Now).Sum(x => x.Quantity);
        }
        public List<MedicalSupplyInventory> MedicalSupplyDetail(int medicalSupplyId)
        {
            return _context.MedicalSupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now).ToList();
        }
        public bool AddSupplyInventory(MedicalSupplyInventory supplyInventory)
        {
            try
            {
                _context.MedicalSupplyInventories.Add(supplyInventory);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddRangeSupplyInventory(List<MedicalSupplyInventory> supplyInventories)
        {
            foreach (var item in supplyInventories)
            {
                if (!AddSupplyInventory(item))
                    return false;
            }
            return true;
        }
    }
}