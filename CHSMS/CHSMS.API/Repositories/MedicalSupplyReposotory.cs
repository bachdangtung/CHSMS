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
            return _context.SupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId && x.ExpirationDate > DateTime.Now).Sum(x => x.Quantity);
        }
        public List<SupplyInventory> MedicalSupplyDetail(int medicalSupplyId)
        {
            return _context.SupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId && x.Quantity > 0 && x.ExpirationDate > DateTime.Now).ToList();
        }
        public bool AddSupplyInventory(SupplyInventory supplyInventory)
        {
            try
            {
                _context.SupplyInventories.Add(supplyInventory);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddRangeSupplyInventory(List<SupplyInventory> supplyInventories)
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