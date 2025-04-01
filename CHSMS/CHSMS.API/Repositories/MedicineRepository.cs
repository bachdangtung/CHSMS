using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CHSMS.API.Repositories
{
    public class MedicineRepository
    {
        private readonly SEP_TestContext _context;
        private readonly HttpClient _httpClient;
        private const string SheetID = "12aVbY2ZHYXkXg1CnZ_V_y_Rrw4i8BEoH"; // ID của Google Sheets
        private const string SheetURL = $"https://docs.google.com/spreadsheets/d/{SheetID}/gviz/tq?tqx=out:json"; // URL API Google Sheets
        public MedicineRepository(HttpClient httpClient, SEP_TestContext context)
        {
            _context = context;
            _httpClient = httpClient;
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
        public List<MedicineInventoryDTO> GetMedicineInventoryByMedicineId(int medicineId)
        {
            var medicineInventories = _context.MedicineInventories
                .Include(m => m.Medicine)
                .Include(m => m.Supplier)
                .Include(m => m.Receiver)
                .Where(m => m.MedicineId == medicineId)
                .Select(m => new MedicineInventoryDTO
                {
                    MedicineInventoryId = m.MedicineInventoryId,
                    MedicineId = m.MedicineId,
                    MedicineName = m.Medicine.MedicineName,
                    CertificateNumber = m.CertificateNumber,
                    TransactionType = m.TransactionType,
                    Quantity = m.Quantity,
                    ManufacturingDate = m.ManufacturingDate,
                    ExpiryDate = m.ExpiryDate,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver != null ? m.Receiver.UserName : null, // Lấy tên từ bảng User
                    TransactionDate = m.TransactionDate,
                    Note = m.Note,
                    BatchNumber = m.BatchNumber,
                    SupplierId = m.SupplierId,
                    SupplierName = m.Supplier != null ? m.Supplier.Name : null // Lấy tên từ bảng Supplier
                })
                .ToList();

            return medicineInventories;
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

        // Get total quantity of a medicine
        public double GetMedicineQuantity(int medicineId)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0)
                .Sum(x => x.Quantity) ?? 0;
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

        //search medicine by name
        public List<Medicine> SearchMedicineByName(string name)
        {
            return _context.Medicines
                .Include(m => m.MedicineInventories)
                .ThenInclude(mi => mi.Supplier)
                .Where(m => m.MedicineName.StartsWith(name)) // Chỉ lấy thuốc bắt đầu bằng 'name'
                .ToList();
        }


        // Add medicine inventory
        public bool AddMedicineInventory(MedicineInventory medicineInventory)
        {
            var medicineExists = _context.Medicines.Any(m => m.MedicineId == medicineInventory.MedicineId);
            if (!medicineExists)
            {
                throw new Exception("Thuốc không tồn tại trong hệ thống.");
            }

            _context.MedicineInventories.Add(medicineInventory);
            return _context.SaveChanges() > 0;
        }

        // Update medicine inventory
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
            // Cập nhật thêm các trường cần thiết

            return _context.SaveChanges() > 0;
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

        // Tìm kiếm thuốc theo nhiều tiêu chí
        public async Task<List<Medicine>> SearchMedicinesAsync(
    int? medicineId = null,
    string? medicineName = null,
    string? activeIngredient = null,
    string? dosage = null,
    string? dosageForm = null,
    double? quantity = null,
    double? importPrice = null,
    DateTime? expiryDate = null,
    string? batchNumber = null,
    string? bidNumber = null,
    bool? status = null)
        {
            var query = _context.Medicines
                .Include(m => m.MedicineInventories)
                .ThenInclude(mi => mi.Supplier)
                .AsQueryable();

            // Áp dụng các bộ lọc
            if (medicineId.HasValue)
            {
                query = query.Where(m => m.MedicineId == medicineId.Value);
            }

            if (!string.IsNullOrWhiteSpace(medicineName))
            {
                query = query.Where(m => EF.Functions.Like(m.MedicineName, $"{medicineName}%"));
            }

            if (!string.IsNullOrWhiteSpace(activeIngredient))
            {
                query = query.Where(m => EF.Functions.Like(m.ActiveIngredient, $"{activeIngredient}%"));
            }

            if (!string.IsNullOrWhiteSpace(dosage))
            {
                query = query.Where(m => EF.Functions.Like(m.Dosage, $"{dosage}%"));
            }

            if (!string.IsNullOrWhiteSpace(dosageForm))
            {
                query = query.Where(m => EF.Functions.Like(m.DosageForm, $"{dosageForm}%"));
            }

            if (importPrice.HasValue)
            {
                query = query.Where(m => m.ImportPrice.ToString().StartsWith(importPrice.Value.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(bidNumber))
            {
                query = query.Where(m => EF.Functions.Like(m.BidNumber, $"{bidNumber}%"));
            }

            // Lọc theo các thuộc tính trong MedicineInventories
            if (quantity.HasValue)
            {
                query = query.Where(m => m.MedicineInventories.Any(mi => mi.Quantity.ToString().StartsWith(quantity.Value.ToString())));
            }

            if (expiryDate.HasValue)
            {
                query = query.Where(m => m.MedicineInventories.Any(mi => mi.ExpiryDate.HasValue && mi.ExpiryDate.Value.Date == expiryDate.Value.Date));
            }

            if (!string.IsNullOrWhiteSpace(batchNumber))
            {
                query = query.Where(m => m.MedicineInventories.Any(mi => EF.Functions.Like(mi.BatchNumber, $"{batchNumber}%")));
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status.ToString().StartsWith(status.Value.ToString()));
            }

            var medicines = await query.ToListAsync();
            if (medicines == null || medicines.Count == 0)
            {
                return new List<Medicine>(); // Trả về danh sách rỗng
            }
            return medicines;
        }


        // Search for medicines
        public async Task<dynamic> SearchMedicinesData(string query)
        {
            // For searching, we'll still get all data and filter it in the service
            var response = await _httpClient.GetStringAsync(SheetURL);
            return ParseGoogleSheetsResponse(response);
        }
        // Parse Google Sheets API response
        private dynamic ParseGoogleSheetsResponse(string response)
        {
            // Google Sheets API returns data wrapped in a callback function, we need to extract the JSON
            string jsonString = Regex.Match(response, @"(?<=\().*(?=\);)").Value;
            return JObject.Parse(jsonString);
        }
    }
}
