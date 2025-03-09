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

        //Get all medical supplies
        public List<MedicalSupply> GetAllMedicalSupplies()
        {
            return _context.MedicalSupplies.ToList();
        }

        //Get one medical supply by ID
        public MedicalSupply? GetMedicalSupply(int medicalSupplyId)
        {
            var result = _context.MedicalSupplies.Find(medicalSupplyId);
            if (result == null)
                return null;
            return result;
        }

        //Get supply total quantity
        public double? GetSupplyQuantity(int medicalSupplyId)
        {
            double sum = 0;
            var supplyInventory = GetAvailableMedicalSupplyInventory(medicalSupplyId);
            foreach (var item in supplyInventory)
            {
                sum += item.Quantity.Value;
            }
            return sum;
        }


        //Get supply detail
        public List<MedicalSupplyInventory> MedicalSupplyDetail(int medicalSupplyId)
        {
            return _context.MedicalSupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now).ToList();
        }

        //Add medical supply inventory      
        public bool AddMedicalSupplyInventory(MedicalSupplyInventory medicalSupply)
        {
            _context.MedicalSupplyInventories.Add(medicalSupply);
            return _context.SaveChanges() > 0;
        }

        //Update medical supply inventory
        public bool UpdateMedicalSupplyInventory(MedicalSupplyInventory medicalSupplyInventory)
        {
            _context.MedicalSupplyInventories.Update(medicalSupplyInventory);
            return (_context.SaveChanges() > 0);
        }

        //Consume medical supply inventory
        public int ConsumeMedicalSupply(int id, double Quantity, bool BHYT, string? Note)
        {
            var supplyInventory = GetAvailableMedicalSupplyInventory(id);
            var medicalSupplyConsumption = new List<MedicalSupplyConsumption>();
            foreach (var item in supplyInventory)
            {
                if (item.Quantity < Quantity)
                {
                    item.Quantity = 0;
                    Quantity -= item.Quantity.Value;
                    medicalSupplyConsumption.Add(new MedicalSupplyConsumption
                    {
                        Msid = item.SupplyInventoryId,
                        Amount = item.Quantity.Value,
                        ConsumptionDate = DateTime.Now,
                        Bhyt = BHYT,
                        Note = Note
                    });
                }
                else
                {
                    medicalSupplyConsumption.Add(new MedicalSupplyConsumption
                    {
                        Msid = item.SupplyInventoryId,
                        Amount = Quantity,
                        ConsumptionDate = DateTime.Now,
                        Bhyt = BHYT,
                        Note = Note
                    });
                    item.Quantity -= Quantity;
                    Quantity = 0;
                }
                if (Quantity == 0)
                    break;
            }
            _context.MedicalSupplyInventories.UpdateRange(supplyInventory);
            _context.MedicalSupplyConsumptions.AddRange(medicalSupplyConsumption);
            if (!(_context.SaveChanges() > 0))
                return 0;
            return 1;
        }

        //Get all available medical supply inventory by medical supply ID
        public List<MedicalSupplyInventory> GetAvailableMedicalSupplyInventory(int medicalSupplyId)
        {
            return _context.MedicalSupplyInventories
                .Where(x => x.MedicalSupplyId == medicalSupplyId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        //Add medical supply consumption
        public bool AddMedicalSupplyConsumption(MedicalSupplyConsumption medicalSupplyConsumption)
        {
            _context.MedicalSupplyConsumptions.Add(medicalSupplyConsumption);
            return _context.SaveChanges() > 0;
        }
    }
}