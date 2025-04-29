using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly SEP_TestContext _context;
        private readonly HttpClient _httpClient;
        public MedicineRepository(HttpClient httpClient, SEP_TestContext context)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public List<MedicineInventory> GetAllMedicineInventories()
        {
            return _context.MedicineInventories
                .Include(mi => mi.Medicine)
                .Include(mi => mi.Supplier)
                .ToList();
        }

        // Get all medicines
        public List<Medicine> GetAllMedicine()
        {
            var medicines = _context.Medicines
                .Include(m => m.MedicineInventories)
                .ThenInclude(mi => mi.Supplier)
                .ToList();
            return medicines;
        }

        //Get medicineInventory by medicineId
        public List<MedicineInventoryDetailDTO> GetMedicineInventoryByMedicineId(int medicineId)
        {
            var medicineInventories = _context.MedicineInventories
                .Include(m => m.Medicine)
                .Include(m => m.Supplier)
                .Include(m => m.Receiver)
                .Where(m => m.MedicineId == medicineId)
                .Select(m => new MedicineInventoryDetailDTO
                {
                    MedicineInventoryId = m.MedicineInventoryId,
                    MedicineId = m.MedicineId,
                    MedicineName = m.Medicine.MedicineName,
                    CertificateNumber = m.CertificateNumber,
                    TransactionType = m.TransactionType,
                    Quantity = m.Quantity,
                    ImportQuantity = m.ImportQuantity,
                    ManufacturingDate = m.ManufacturingDate,
                    ExpiryDate = m.ExpiryDate,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver != null ? m.Receiver.UserName : null,
                    TransactionDate = m.TransactionDate,
                    Note = m.Note,
                    BatchNumber = m.BatchNumber,
                    SupplierId = m.SupplierId,
                    SupplierName = m.Supplier != null ? m.Supplier.Name : null
                })
                .ToList();

            return medicineInventories;
        }

        public Medicine? GetMedicineByMedicineInventoryId(int id)
        {
            var medicineInventory = _context.MedicineInventories
                .Where(x => x.MedicineInventoryId == id)
                .FirstOrDefault();
            return _context.Medicines
                .Where(x => x.MedicineId == medicineInventory.MedicineId)
                .FirstOrDefault();
        }

        public MedicineInventory? GetMedicineInventoryById(int id)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineInventoryId == id)
                .FirstOrDefault();  
        }

        public MedicineConsumption? GetMedicineConsumptionById(int id)
        {
            return _context.MedicineConsumptions
                .Where(x => x.MedicineConsumptionId == id)
                .FirstOrDefault();
        }

        public bool UpdateMedicineConsumption(MedicineConsumption medicineConsumption)
        {
            _context.MedicineConsumptions.Update(medicineConsumption);
            return _context.SaveChanges() > 0;
        }

        public List<MedicineInventory> GetMedicineImportHistory(DateTime fromDate, DateTime toDate)
        {
            return _context.MedicineInventories
                .Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate)
                .ToList();
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _context.Suppliers.ToList();
        }

        // Get all medicines with pagination
        public Medicine? GetMedicine(int medicineId)
        {
            var result = _context.Medicines
                                 .Include(m => m.MedicineInventories)
                                 .ThenInclude(mi => mi.Supplier)
                                 .FirstOrDefault(m => m.MedicineId == medicineId);

            return result;
        }

        // Get list medicine by medicineId còn hạn sử dụng
        public List<MedicineInventory> GetMedicineInventory(int medicineId, bool orderByExpiry = false)
        {
            var query = _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now);

            return orderByExpiry ? query.OrderBy(x => x.ExpiryDate).ToList() : query.ToList();
        }

        // Get list medicine by ID
        public List<MedicineInventory> GetAvailableMedicineInventory(int medicineId)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        public List<MedicineInventory> GetAllMedicineInventory(int medicineId)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        //Get actual supply quantity by Date
        public double? GetActualMedicineQuantity(int medicineId, DateTime date)
        {
            double sum = GetMedicineQuantityById(medicineId).Value;
            sum += MedicineConsumeReport(medicineId, date, DateTime.Now);
            sum -= GetInputAmountOfMedicine(medicineId, date, DateTime.Now).Value;
            sum += GetNumberOfExpiredMedicine(medicineId, date, DateTime.Now);
            return sum;
        }

        //Get input amount by time
        public double? GetInputAmountOfMedicine(int medicineId, DateTime? from, DateTime? to)
        {
            double sum = 0;
            var list = GetInputMedicineInventoryByDate(from, to).Where(x => x.MedicineId == medicineId);
            foreach (var item in list)
            {
                sum += item.ImportQuantity.Value;
            }
            return sum;
        }

        //Get input medical supply inventory by time
        public List<MedicineInventory> GetInputMedicineInventoryByDate(DateTime? from, DateTime? to)
        {
            return _context.MedicineInventories
                .Where(x => x.TransactionDate >= from && x.TransactionDate <= to)
                .ToList();
        }

        public double GetNumberOfExpiredMedicine(int medicineId, DateTime? from, DateTime? to)
        {
            double sum = 0;
            sum += _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.ExpiryDate <= DateTime.Now && x.ExpiryDate >= from)
                .Sum(x => x.Quantity).Value;
            return sum;
        }

        //Consume medicine inventory
        public int ConsumeMedicineByMedicineId(ConsumeMedicineDTO consumeMedicineDTO)
        {
            MedicineConsumption medicineConsumption = new MedicineConsumption
            {
                MedicineInventoryId = consumeMedicineDTO.MedicineInventoryId.Value,
                Amount = consumeMedicineDTO.Quantity,
                ConsumptionDate = DateTime.Now,
                Status = consumeMedicineDTO.Status,
                Note = consumeMedicineDTO.Note
            };
            _context.MedicineConsumptions.Add(medicineConsumption);
            if (!(_context.SaveChanges() > 0))
                return 0;
            return 1;
        }

        //Get all medical supply consumption report
        public Dictionary<Medicine, double> GetAllMedicineConsumeReport(DateTime? from, DateTime? to)
        {
            var result = new Dictionary<Medicine, double>();
            var list = GetAllMedicine();
            foreach (var item in list)
            {
                result.Add(item, MedicineConsumeReport(item.MedicineId, from, to));
            }
            return result;
        }

        // Get total quantity of a medicine
        public double GetMedicineQuantity(int medicineId)
        {
            double sum = 0;
            var medicineInventory = GetAllMedicineInventory(medicineId);
            foreach (var item in medicineInventory)
            {
                sum += item.Quantity.Value;
            }
            return sum;
        }

        //Get supply total quantity
        public double? GetMedicineQuantityById(int medicineId)
        {
            double sum = 0;
            var medicineInventory = GetAvailableMedicineInventory(medicineId);
            foreach (var item in medicineInventory)
            {
                sum += item.Quantity.Value;
            }
            return sum;
        }

        public double GetAddOnMedicineInventory(int id, DateTime? from, DateTime? to)
        {
            var result = _context.MedicineInventories
                .Where(x => x.MedicineId == id && x.TransactionDate >= from && x.TransactionDate <= to)
                .Sum(x => x.ImportQuantity);
            return result.Value;
        }

        public double GetNumberOfExpiredMedicineInventory(int medicineInventoryId, DateTime? from, DateTime? to)
        {
            double sum = 0;
            sum += _context.MedicineInventories
                .Where(x => x.MedicineId == medicineInventoryId && x.ExpiryDate <= DateTime.Now && x.ExpiryDate >= from)
                .Sum(x => x.Quantity).Value;
            return sum;
        }

        public List<MedicineConsumption> MedicineConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _context.MedicineConsumptions
                .Where(x => x.MedicineInventoryId == id && x.ConsumptionDate >= from && x.ConsumptionDate <= to && x.Status == true)
                .ToList();
            return result;
        }

        public List<MedicineConsumption> ConsumptionHistory(DateTime? from, DateTime? to)
        {
            return _context.MedicineConsumptions
                 .Where(x => x.ConsumptionDate >= from && x.ConsumptionDate <= to && x.Status == true)
                 .ToList();
        }

        //Get medical supply consumption report by MSID
        public double MedicineConsumeReport(int medicineid, DateTime? from, DateTime? to)
        {
            double sum = 0;
            var listconsumption = GetAllMedicineConsumptionByDate(from, to);
            var listinventory = GetAllMedicineInventory(medicineid);
            foreach (var item in listinventory)
            {
                listconsumption.Where(x => x.MedicineInventoryId == item.MedicineInventoryId)
                    .Sum(x => sum += x.Amount.Value);
            }
            return sum;
        }

        //Get medical supply consumption by time
        public List<MedicineConsumption> GetAllMedicineConsumptionByDate(DateTime? from, DateTime? to)
        {
            return _context.MedicineConsumptions
                .Where(x => x.ConsumptionDate >= from && x.ConsumptionDate <= to && x.Status == true)
                .ToList();
        }

        // Get total quantity of a medicine
        public DateTime? CalculateExpiryDate(DateTime? manufacturingDate, int? shelfLife)
        {
            if (manufacturingDate.HasValue && shelfLife.HasValue)
            {
                return manufacturingDate.Value.AddMonths(shelfLife.Value);
            }
            return null;
        }

        // Add medicine inventory
        public bool AddMedicineInventoryList(List<MedicineInventory> inventoryList)
        {
            foreach (var item in inventoryList)
            {
                var medicineExists = _context.Medicines.Any(m => m.MedicineId == item.MedicineId);
                if (!medicineExists)
                    throw new Exception($"Thuốc có ID {item.MedicineId} không tồn tại.");
            }

            _context.MedicineInventories.AddRange(inventoryList);
            return _context.SaveChanges() > 0;
        }

        // Update medicine inventory (bỏ)
        public bool UpdateMedicineInventory(MedicineInventory medicineInventory)
        {
            var existingInventory = _context.MedicineInventories
                .FirstOrDefault(mi => mi.MedicineInventoryId == medicineInventory.MedicineInventoryId);

            if (existingInventory == null)
            {
                throw new Exception("Kho thuốc không tồn tại.");
            }

            // Chỉ cập nhật những trường thay đổi
            existingInventory.CertificateNumber = medicineInventory.CertificateNumber;
            existingInventory.Quantity = medicineInventory.Quantity;
            existingInventory.TransactionType = medicineInventory.TransactionType;
            existingInventory.ReceiverId = medicineInventory.ReceiverId;
            existingInventory.TransactionDate = medicineInventory.TransactionDate;
            existingInventory.Note = medicineInventory.Note;
            existingInventory.BatchNumber = medicineInventory.BatchNumber;
            existingInventory.Quantity = medicineInventory.Quantity;
            existingInventory.ExpiryDate = medicineInventory.ExpiryDate;
            existingInventory.ManufacturingDate = medicineInventory.ManufacturingDate;
            existingInventory.SupplierId = medicineInventory.SupplierId;

            return _context.SaveChanges() > 0;
        }

        public bool UpdateMedicineInInventory(List<MedicineInventory> medicineInventory)
        {

            _context.MedicineInventories.UpdateRange(medicineInventory);
            return (_context.SaveChanges() > 0);
        }


        // Lấy danh sách thuốc sắp hết hạn (ví dụ: trong vòng 6 tháng tới)
        public List<MedicineInventory> GetNearExpiryMedicines(int monthsThreshold = 6)
        {
            var thresholdDate = DateTime.Now.AddMonths(monthsThreshold);
            return _context.MedicineInventories
                .Include(mi => mi.Medicine)
                .Include(mi => mi.Supplier)
                .Where(x => x.Quantity > 0 && x.ExpiryDate <= thresholdDate && x.ExpiryDate > DateTime.Now)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        // Lấy danh sách thuốc dưới ngưỡng tồn kho tối thiểu
        public List<Medicine> GetLowStockMedicines(double minimumThreshold)
        {
            return _context.Medicines
                .Where(m => m.MedicineInventories
                    .Where(mi => mi.ExpiryDate > DateTime.Now)
                    .Sum(mi => mi.Quantity) < minimumThreshold)
                .Include(m => m.MedicineInventories)
                .ToList();
        }

        // Đánh dấu thuốc đã hết hạn sử dụng
        public List<MedicineInventory> GetExpiredMedicines()
        {
            return _context.MedicineInventories
                .Include(mi => mi.Medicine)
                .Include(mi => mi.Supplier)
                .Where(x => x.Quantity > 0 && x.ExpiryDate <= DateTime.Now)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        // Lấy danh sách thuốc theo lô
        public List<MedicineInventory> GetMedicinesByBatchNumber(string batchNumber)
        {
            return _context.MedicineInventories
                .Include(mi => mi.Medicine)
                .Include(mi => mi.Supplier)
                .Where(x => x.BatchNumber == batchNumber)
                .ToList();
        }

        //search medicine by name
        public List<Medicine> SearchMedicineByName(string name)
        {
            return _context.Medicines
                .Include(m => m.MedicineInventories)
                .ThenInclude(mi => mi.Supplier)
                .Where(m => m.MedicineName.StartsWith(name)) // Chỉ lấy thuốc bắt đầu bằng 'name'
                .ToList();
        }

        // Tìm kiếm thuốc theo nhiều tiêu chí
        public async Task<List<MedicineInventory>> SearchMedicinesAsync(
    int? medicineId = null, string? medicineName = null,
    string? activeIngredient = null, string? dosage = null,
    string? dosageForm = null, double? quantity = null,
    double? importPrice = null,
    DateTime? expiryDate = null, string? batchNumber = null,
    string? bidNumber = null, bool? status = null,
    DateTime? minExpiryDate = null, DateTime? maxExpiryDate = null)
        {
            var query = _context.MedicineInventories
                .Include(mi => mi.Medicine)
                .Include(mi => mi.Supplier)
                .AsQueryable();

            if (medicineId.HasValue && medicineId.Value > 0)
            {
                query = query.Where(mi => mi.MedicineId == medicineId.Value);
            }

            if (!string.IsNullOrWhiteSpace(medicineName))
            {
                query = query.Where(mi => EF.Functions.Like(mi.Medicine.MedicineName, $"{medicineName}%"));
            }

            if (!string.IsNullOrWhiteSpace(activeIngredient))
            {
                query = query.Where(mi => EF.Functions.Like(mi.Medicine.ActiveIngredient, $"{activeIngredient}%"));
            }

            if (!string.IsNullOrWhiteSpace(dosage))
            {
                query = query.Where(mi => EF.Functions.Like(mi.Medicine.Dosage, $"{dosage}%"));
            }

            if (!string.IsNullOrWhiteSpace(dosageForm))
            {
                query = query.Where(mi => EF.Functions.Like(mi.Medicine.DosageForm, $"{dosageForm}%"));
            }

            if (importPrice.HasValue)
            {
                query = query.Where(mi => mi.Medicine.ImportPrice == importPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(bidNumber))
            {
                query = query.Where(mi => EF.Functions.Like(mi.Medicine.BidNumber, $"{bidNumber}%"));
            }

            if (quantity.HasValue)
            {
                query = query.Where(mi => mi.Quantity == quantity.Value);
            }

            if (expiryDate.HasValue)
            {
                query = query.Where(mi =>
                    mi.ExpiryDate.HasValue &&
                    mi.ExpiryDate.Value.Date == expiryDate.Value.Date);
            }
            else
            {
                if (minExpiryDate.HasValue)
                {
                    query = query.Where(mi =>
                        mi.ExpiryDate.HasValue &&
                        mi.ExpiryDate.Value.Date >= minExpiryDate.Value.Date);
                }

                if (maxExpiryDate.HasValue)
                {
                    query = query.Where(mi =>
                        mi.ExpiryDate.HasValue &&
                        mi.ExpiryDate.Value.Date <= maxExpiryDate.Value.Date);
                }
            }

            if (!string.IsNullOrWhiteSpace(batchNumber))
            {
                query = query.Where(mi => EF.Functions.Like(mi.BatchNumber, $"{batchNumber}%"));
            }

            if (status.HasValue)
            {
                query = query.Where(mi => mi.Medicine.Status == status.Value);
            }

            query = query.OrderBy(mi => mi.MedicineInventoryId);

            var medicines = await query.ToListAsync();
            return medicines ?? new List<MedicineInventory>();
        }


        public List<MedicineDTO> GetFilteredMedicineInventory(MedicineInventoryFilter filter)
        {
            var virtualStockDict = GetVirtualStock();
            var actualStockDict = GetActualStock();
            var medicines = GetAllMedicine();

            var medicineIds = medicines.Select(m => m.MedicineId).ToList();
            var minimumStockDict = GetMinimumStock(medicineIds, filter.MinimumStock);

            var result = new List<MedicineDTO>();

            foreach (var medicine in medicines)
            {
                var virtualQty = virtualStockDict.TryGetValue(medicine.MedicineId, out var vQty) ? vQty : 0;
                var actualQty = actualStockDict.TryGetValue(medicine.MedicineId, out var aQty) ? aQty : 0;
                var minimumQty = minimumStockDict.TryGetValue(medicine.MedicineId, out var minQty) ? minQty : 0;

                bool passesFilter = true;

                // Filter 1: Theo tồn kho thực
                if (filter.ViewActualStock && actualQty <= 0) passesFilter = false;

                // Filter 2: Theo tồn kho ảo
                if (filter.ViewVirtualStock && virtualQty <= 0) passesFilter = false;

                // Filter 3: Theo tồn tối thiểu (độc lập)
                if (filter.MinimumStock != null)
                {
                    if (actualQty < minimumQty && virtualQty < minimumQty)
                        passesFilter = false;
                }

                if (passesFilter)
                {
                    var firstInventory = medicine.MedicineInventories.FirstOrDefault();
                    result.Add(new MedicineDTO
                    {
                        MedicineId = medicine.MedicineId,
                        MedicineName = medicine.MedicineName,
                        MedicineCode = medicine.MedicineCode,
                        ActiveIngredient = medicine.ActiveIngredient,
                        Dosage = medicine.Dosage,
                        DosageForm = medicine.DosageForm,
                        ImportPrice = medicine.ImportPrice,
                        SellingPrice = medicine.SellingPrice,
                        Quantity = actualQty,
                        ShelfLife = medicine.ShelfLife,
                        BatchNumber = firstInventory?.BatchNumber,
                        BidNumber = medicine.BidNumber,
                        IsBhyt = medicine.IsBhyt,
                        ManufacturingDate = firstInventory?.ManufacturingDate,
                        ExpiryDate = firstInventory?.ExpiryDate,
                        Status = medicine.Status
                    });
                }
            }

            return result;
        }

        private Dictionary<int, double> GetVirtualStock()
        {
            return _context.MedicineInventories
                .GroupBy(i => i.MedicineId)
                .Select(g => new
                {
                    MedicineId = g.Key,
                    TotalImportQuantity = g.Sum(i => i.ImportQuantity ?? 0)
                })
                .ToDictionary(x => x.MedicineId, x => x.TotalImportQuantity);
        }

        private Dictionary<int, double> GetActualStock()
        {
            return _context.MedicineInventories
                .GroupBy(i => i.MedicineId)
                .Select(g => new
                {
                    MedicineId = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity ?? 0)
                })
                .ToDictionary(x => x.MedicineId, x => x.TotalQuantity);
        }
        private Dictionary<int, double> GetMinimumStock(List<int> medicineIds, double? minimumStock)
        {
            double threshold = minimumStock ?? 0;

            return medicineIds.ToDictionary(id => id, id => threshold);
        }
        public bool CheckDuplicateBatch(int medicineId, string? batchNumber, DateTime? transactionDate)
        {
            if (string.IsNullOrEmpty(batchNumber) || transactionDate == null)
                return false;

            return _context.MedicineInventories.Any(m =>
                m.MedicineId == medicineId &&
                m.BatchNumber == batchNumber &&
                m.TransactionDate.Value.Date == transactionDate.Value.Date
            );
        }
        public MedicineInventory GetInventoryById(int inventoryId)
        {
            return _context.MedicineInventories.FirstOrDefault(x => x.MedicineInventoryId == inventoryId);
        }
        // Repo implementation
        public List<MedicineInventory> GetRecentInventoriesByUser(int userId)
        {
            return _context.MedicineInventories
                .Where(x => x.ReceiverId == userId && x.TransactionDate >= DateTime.Now.AddDays(-1))
                .ToList();
        }

        public List<MedicineInventory> GetAllInventoriesByUser(int userId)
        {
            return _context.MedicineInventories
                .Where(x => x.ReceiverId == userId)
                .OrderByDescending(x => x.TransactionDate)
                .ToList();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
