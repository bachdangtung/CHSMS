using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CHSMS.API.Repositories
{
    public class MedicalSupplyRepository : IMedicalSupplyRepository
    {
        private readonly SEP_TestContext _context;
        public MedicalSupplyRepository(SEP_TestContext context)
        {
            _context = context;
        }

        //MS :MedicalSupply
        //MSI:MedicalSupplyInventory
        //MSC:MedicalSupplyConsumption

        //Get all medical supplies
        public List<MedicalSupply> GetAllMedicalSupplies()
        {
            return _context.MedicalSupplies.ToList();
        }

        //Get all MSI by MedicalSupplyId
        public List<MedicalSupplyInventory>? GetMedicalSupplyInventoryByMSID(int medicalSupplyId)
        {
            var result = _context.MedicalSupplyInventories.Where(x => x.MedicalSupplyId == medicalSupplyId).ToList();
            if (result == null)
                return null;
            return result;
        }

        //Get supply total quantity
        public double? GetMSQantityByID(int medicalSupplyId)
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
        public double? GetActualMSQuantity(int medicalSupplyId, DateTime date)
        {
            double sum = GetMSQantityByID(medicalSupplyId).Value;
            sum += MedicalSupplyConsumeReport(medicalSupplyId, date, DateTime.Now);
            sum -= GetInputAmountOfMS(medicalSupplyId, date, DateTime.Now).Value;
            sum += GetNumberOfExpiredMSI(medicalSupplyId, date, DateTime.Now);
            return sum;
        }
        //Add medical supply inventory      
        public bool AddMedicalSupplyInventory(List<MedicalSupplyInventory> medicalSupply)
        {
            try
            {
                _context.MedicalSupplyInventories.AddRange(medicalSupply);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Update medical supply inventory
        public bool UpdateMedicalSupplyInventory(List<MedicalSupplyInventory> medicalSupplyInventory)
        {

            _context.MedicalSupplyInventories.UpdateRange(medicalSupplyInventory);
            return (_context.SaveChanges() > 0);
        }

        //Consume medical supply inventory
        public int ConsumeMedicalSupplyByMSID(ConsumpMSDTO consump)
        {
            if (consump.Quantity <= 0)
            {
                throw new Exception("Số lượng không hợp lệ");
            }
            var msInventory= GetMedicalSupplyInventoryById(consump.MedicalSupplyInventoryId.Value);
            if (msInventory == null)
            {
                throw new Exception("Không tìm thấy thông tin tồn kho");
            }
            if (msInventory.Quantity < consump.Quantity)
            {
                throw new Exception("Số lượng tiêu thụ lớn hơn số lượng tồn kho");
            }
            MedicalSupplyConsumption medicalSupplyConsumption = new MedicalSupplyConsumption
            {
                MedicalSupplyInventoryId = consump.MedicalSupplyInventoryId.Value,
                Amount = consump.Quantity,
                ConsumptionDate = DateTime.Now,
                Status = consump.Status,
                Note = consump.Note
            };
            _context.MedicalSupplyConsumptions.Add(medicalSupplyConsumption);
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
        public Dictionary<MedicalSupply, double> GetAllMedicalSupplyConsumeReport(DateTime? from, DateTime? to)
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
            var listconsumption = GetAllMedicalSupplyConsumptionByDate(from, to);
            var listinventory = GetAllMedicalSupplyInventory(msid);
            foreach (var item in listinventory)
            {
                listconsumption.Where(x => x.MedicalSupplyInventoryId == item.SupplyInventoryId)
                    .Sum(x => sum += x.Amount.Value);
            }
            return sum;
        }

        //Get medical supply consumption by time
        public List<MedicalSupplyConsumption> GetAllMedicalSupplyConsumptionByDate(DateTime? from, DateTime? to)
        {
            return _context.MedicalSupplyConsumptions
                .Where(x => x.ConsumptionDate >= from && x.ConsumptionDate <= to && x.Status == true)
                .ToList();
        }

        //Get input medical supply inventory by time
        public List<MedicalSupplyInventory> GetInputMedicalSupplyInventoryByDate(DateTime? from, DateTime? to)
        {
            return _context.MedicalSupplyInventories
                .Where(x => x.TransactionDate >= from && x.TransactionDate <= to)
                .ToList();
        }
        //Get input amount by time
        public double? GetInputAmountOfMS(int MSID, DateTime? from, DateTime? to)
        {
            double sum = 0;
            var list = GetInputMedicalSupplyInventoryByDate(from, to).Where(x => x.MedicalSupplyId == MSID);
            foreach (var item in list)
            {
                sum += item.ImportQuantity.Value;
            }
            return sum;
        }

        public MedicalSupply GetMedicalSupplyByID(int id)
        {
            var result = _context.MedicalSupplies
                .Where(x => x.MedicalSupplyId == id)
                .FirstOrDefault();
            if (result == null)
                return null;
            return result;
        }

        public List<MedicalSupplyConsumption> MSConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _context.MedicalSupplyConsumptions
                .Where(x => x.MedicalSupplyInventoryId == id && x.ConsumptionDate >= from && x.ConsumptionDate <= to && x.Status == true)
                .ToList();
            return result;
        }

        public double GetAddOnMSI(int id, DateTime? from, DateTime? to)
        {
            var result = _context.MedicalSupplyInventories
                .Where(x => x.MedicalSupplyId == id && x.TransactionDate >= from && x.TransactionDate <= to)
                .Sum(x => x.ImportQuantity);
            return result.Value;
        }

        public List<MedicalSupplyConsumption> ConsumptionHistory(DateTime? from, DateTime? to)
        {
            return _context.MedicalSupplyConsumptions
                 .Where(x => x.ConsumptionDate >= from && x.ConsumptionDate <= to && x.Status == true)
                 .ToList();
        }

        public MedicalSupply? GetMedicalSupplyByMSIID(int id)
        {
            
            return _context.MedicalSupplies
                .AsNoTracking()
                .Where(x => x.MedicalSupplyId == id)
                .FirstOrDefault();
        }
        public MedicalSupplyConsumption? GetSupplyConsumptionByID(int id)
        {
            return _context.MedicalSupplyConsumptions
                .Where(x => x.MsconsumptionId == id)
                .FirstOrDefault();
        }

        public MedicalSupplyInventory? GetMedicalSupplyInventoryById(int id)
        {
            return _context.MedicalSupplyInventories.AsNoTracking()
                .Where(x => x.SupplyInventoryId == id)
                .FirstOrDefault();
        }
        public bool UpdateMedicalSupplyConsumption(MedicalSupplyConsumption medicalSupplyConsumption)
        {
            _context.MedicalSupplyConsumptions.Update(medicalSupplyConsumption);
            return _context.SaveChanges() > 0;
        }
        public double GetNumberOfExpiredMSI(int MSID, DateTime? from, DateTime? to)
        {
            double sum = 0;
            sum += _context.MedicalSupplyInventories
                .Where(x => x.MedicalSupplyId == MSID && x.ExpiryDate <= DateTime.Now && x.ExpiryDate >= from)
                .Sum(x => x.Quantity).Value;
            return sum;
        }

        public List<MedicalSupplyInventory> GetMedicalSupplyImportHistory(DateTime fromDate, DateTime toDate)
        {
            return _context.MedicalSupplyInventories
                .Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate)
                .ToList();
        }
        public List<MedicalSupplyInventoryStatistic> GetAllMedicalSupplyInventoryStatistics()
        {
            return _context.MedicalSupplyInventoryStatistics.ToList();
        }

        public MedicalSupplyInventoryStatistic? GetMedicalSupplyInventoryStatisticById(int id)
        {
            return _context.MedicalSupplyInventoryStatistics
                .Where(x => x.Msisid == id)
                .FirstOrDefault();
        }
        public List<MedicalSupplyInventoryStatistic> GetMedicalSupplyInventoryStatisticsByMSIId(int id)
        {
            return _context.MedicalSupplyInventoryStatistics
                .Where(x => x.MsinventoryId == id)
                .ToList();
        }
        public bool AddMedicalSupplyInventoryStatistic(List<MedicalSupplyInventoryStatistic> medicalSupplyInventoryStatistic)
        {
            _context.MedicalSupplyInventoryStatistics.AddRange(medicalSupplyInventoryStatistic);
            return _context.SaveChanges() > 0;
        }
        public bool UpdateMedicalSupplyInventoryStatistic(MedicalSupplyInventoryStatistic medicalSupplyInventoryStatistic)
        {
            _context.MedicalSupplyInventoryStatistics.Update(medicalSupplyInventoryStatistic);
            return _context.SaveChanges() > 0;
        }
        public List<MedicalSupplyInventoryStatistic>? GetMedicalSupplyInventoryStatisticsByStatisticDate(DateTime from, DateTime to)
        {
            return _context.MedicalSupplyInventoryStatistics
                .Where(x => x.StatisticDate >= from && x.StatisticDate <= to)
                .ToList();
        }
        public List<MedicalSupplyInventoryStatistic>? GetMedicalSupplyInventoryStatisticsByConfirmDate(DateTime from, DateTime to)
        {
            return _context.MedicalSupplyInventoryStatistics
                .Where(x => x.ConfirmDate >= from && x.ConfirmDate <= to)
                .ToList();
        }
        public bool DeleteMedicalSupplyInventoryStatistic(MedicalSupplyInventoryStatistic medicalSupplyInventoryStatistic)
        {
            _context.MedicalSupplyInventoryStatistics.Remove(medicalSupplyInventoryStatistic);
            return _context.SaveChanges() > 0;
        }
        public bool UpdateMedicalSupplyInventoryStatistic(List<MedicalSupplyInventoryStatistic> medicalSupplyInventoryStatistics)
        {
            try
            {
                _context.MedicalSupplyInventoryStatistics.UpdateRange(medicalSupplyInventoryStatistics);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<MedicalSupplyInventoryStatistic> GetAllMSISNotConfirm()
        {
            return _context.MedicalSupplyInventoryStatistics
                .Where(x => x.ConfirmDate == null)
                .ToList();
        }

    }
}