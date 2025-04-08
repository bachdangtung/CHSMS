using CHSMS.API.DTOs.MedicalSupply;
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
        public List<MedicalSupplyInventory>? GetMedicalSupplyDetail(int medicalSupplyId)
        {
            var result = _context.MedicalSupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId).ToList();
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
        //Get actual supply quantity by Date
        public double? GetActualSupplyQuantity(int medicalSupplyId, DateTime date)
        {
            double sum = GetSupplyQuantity(medicalSupplyId).Value;
            sum += MedicalSupplyConsumeReport(medicalSupplyId, date, DateTime.Now);
            sum -= GetInputAmount(medicalSupplyId, date, DateTime.Now).Value;
            return sum;
        }

        //Get MedicalSupplyInventory of one MedicalSupply
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
        public int ConsumeMedicalSupply(int id, double Quantity, string? Note)
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
                        MedicalSupplyInventoryId = item.SupplyInventoryId,
                        Amount = item.Quantity.Value,
                        ConsumptionDate = DateTime.Now,
                        Note = Note
                    });
                }
                else
                {
                    medicalSupplyConsumption.Add(new MedicalSupplyConsumption
                    {
                        MsconsumptionId = 0,
                        MedicalSupplyInventoryId = item.SupplyInventoryId,
                        Amount = Quantity,
                        ConsumptionDate = DateTime.Now,
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

        //Get all available medical supply inventory by MSID
        public List<MedicalSupplyInventory> GetAllMedicalSupplyInventory(int msid)
        {
            return _context.MedicalSupplyInventories
                .Where(x => x.MedicalSupplyId == msid)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        //Add medical supply consumption
        public bool AddMedicalSupplyConsumption(MedicalSupplyConsumption medicalSupplyConsumption)
        {
            _context.MedicalSupplyConsumptions.Add(medicalSupplyConsumption);
            return _context.SaveChanges() > 0;
        }

        //Get all medical supply consumption report
        public Dictionary<MedicalSupply, double> MedicalSupplyConsumeReport(DateTime? from, DateTime? to)
        {
            var result = new Dictionary<MedicalSupply, double>();
            var list = GetAllMedicalSupplies();
            foreach (var item in list)
            {
                result.Add(item, MedicalSupplyConsumeReport(item.MedicalSupplyId, from, to));
            }
            return result;
        }

        //Get medical supply consumption report by MSID
        public double MedicalSupplyConsumeReport(int msid, DateTime? from, DateTime? to)
        {
            double sum = 0;
            var listconsumption = GetMedicalSupplyConsumption(from, to);
            var listinventory = GetAllMedicalSupplyInventory(msid);
            foreach (var item in listinventory)
            {
                listconsumption.Where(x => x.MedicalSupplyInventoryId == item.SupplyInventoryId)
                    .Sum(x => sum += x.Amount.Value);
            }
            return sum;
        }

        //Get medical supply consumption by time
        public List<MedicalSupplyConsumption> GetMedicalSupplyConsumption(DateTime? from, DateTime? to)
        {
            return _context.MedicalSupplyConsumptions.Where(x => x.ConsumptionDate >= from && x.ConsumptionDate <= to).ToList();
        }

        //Get input medical supply inventory by time
        public List<MedicalSupplyInventory> GetInputMedicalSupplyInventory(DateTime? from, DateTime? to)
        {
            return _context.MedicalSupplyInventories.Where(x => x.TransactionDate >= from && x.TransactionDate <= to).ToList();
        }
        //Get input amount by time
        public double? GetInputAmount(int MSID, DateTime? from, DateTime? to)
        {
            double sum = 0;
            var list = GetInputMedicalSupplyInventory(from, to).Where(x => x.MedicalSupplyId == MSID);
            foreach (var item in list)
            {
                sum += item.Quantity.Value;
            }
            return sum;
        }

        public MedicalSupply GetMedicalSupply(int id)
        {
            var result = _context.MedicalSupplies.Where(x => x.MedicalSupplyId == id).FirstOrDefault();
            if (result == null)
                return null;
            return result;
        }

        public List<MedicalSupplyConsumption> ConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _context.MedicalSupplyConsumptions
                .Where(x => x.MedicalSupplyInventoryId == id && x.ConsumptionDate >= from && x.ConsumptionDate <= to)
                .ToList();
            return result;
        }

        public double GetAddOnMSI(int id, DateTime? from, DateTime? to)
        {
            var result = _context.MedicalSupplyInventories
                .Where(x => x.MedicalSupplyId == id && x.TransactionDate >= from && x.TransactionDate <= to)
                .Sum(x => x.Quantity);
            return result.Value;
        }

        public List<MedicalSupplyConsumption> ConsumptionHistory(DateTime? from, DateTime? to)
        {
            return _context.MedicalSupplyConsumptions
                 .Where(x => x.ConsumptionDate >= from && x.ConsumptionDate <= to)
                 .ToList();
        }

        public MedicalSupply? GetMedicalSupplyByMSIId(int id)
        {
            var msi = _context.MedicalSupplyInventories
                .Where(x => x.SupplyInventoryId == id)
                .FirstOrDefault();
            return _context.MedicalSupplies
                .Where(x => x.MedicalSupplyId == msi.MedicalSupplyId)
                .FirstOrDefault();
        }
        public MedicalSupplyConsumption? GetSupplyConsumptionById(int id)
        {
            return _context.MedicalSupplyConsumptions
                .Where(x => x.MsconsumptionId == id)
                .FirstOrDefault();
        }

        public MedicalSupplyInventory? GetMedicalSupplyInventoryById(int id)
        {
            return _context.MedicalSupplyInventories
                .Where(x => x.SupplyInventoryId == id)
                .FirstOrDefault();
        }
        public bool UpdateMedicalSupplyConsumption(MedicalSupplyConsumption medicalSupplyConsumption)
        {
            _context.MedicalSupplyConsumptions.Update(medicalSupplyConsumption);
            return _context.SaveChanges() > 0;
        }

    }
}