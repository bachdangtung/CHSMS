using CHSMS.API.DTOs.Medicine;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;
using Newtonsoft.Json.Linq;

namespace CHSMS.API.Services
{
    public class MedicineService
    {
        private readonly SEP_TestContext _context;
        private readonly MedicineRepository _medicineRepository;

        public MedicineService(MedicineRepository medicineRepository, SEP_TestContext context)
        {
            _medicineRepository = medicineRepository;
            _context = context;
        }

        // Get all medicines
        public List<MedicineDTO> GetAll()
        {
            List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();
            foreach (var medicine in _medicineRepository.GetAllMedicine())
            {
                var medicineDTO = new MedicineDTO
                {
                    MedicineId = medicine.MedicineId,
                    MedicineName = medicine.MedicineName,
                    ActiveIngredient = medicine.ActiveIngredient,
                    Dosage = medicine.Dosage,
                    DosageForm = medicine.DosageForm,
                    ImportPrice = medicine.ImportPrice,
                    SellingPrice = medicine.SellingPrice,
                    Quantity = _medicineRepository.GetMedicineQuantity(medicine.MedicineId),
                    ShelfLife = medicine.ShelfLife,
                    BatchNumber = medicine.MedicineInventories.FirstOrDefault()?.BatchNumber,
                    BidNumber = medicine.BidNumber,
                    IsBhyt = medicine.IsBhyt,
                    //SupplierName = medicine.MedicineInventories.FirstOrDefault()?.Supplier?.Name,
                    ManufacturingDate = medicine.MedicineInventories.FirstOrDefault()?.ManufacturingDate,
                    ExpiryDate = medicine.MedicineInventories.FirstOrDefault()?.ExpiryDate,
                    Status = medicine.Status
                };
                medicineDTOs.Add(medicineDTO);
            }
            return medicineDTOs;
        }

        public List<UserDTO> GetAllReceivers()
        {
            return _medicineRepository.GetAllUsers()
                .Select(u => new UserDTO { UserId = u.UserId, UserName = u.UserName })
                .ToList();
        }

        public List<SupplierDTO> GetAllSuppliers()
        {
            return _medicineRepository.GetAllSuppliers()
                .Select(s => new SupplierDTO { SupplierId = s.SupplierId, Name = s.Name })
                .ToList();
        }

        //get all medicine in medicine inventory by medicineId
        public List<MedicineInventoryDTO> GetMedicineInventoryByMedicineId(int medicineId)
        {
            var medicineInventories = _medicineRepository.GetMedicineInventoryByMedicineId(medicineId);

            var medicineInventoryDTOs = medicineInventories.Select(medicineInventory => new MedicineInventoryDTO
            {
                MedicineInventoryId = medicineInventory.MedicineInventoryId,
                MedicineId = medicineInventory.MedicineId,
                MedicineName = medicineInventory.MedicineName,
                Quantity = medicineInventory.Quantity,
                TransactionType = medicineInventory.TransactionType,
                Note = medicineInventory.Note,
                CertificateNumber = medicineInventory.CertificateNumber,
                ExpiryDate = medicineInventory.ExpiryDate,
                ReceiverId = medicineInventory.ReceiverId,
                ReceiverName = medicineInventory.ReceiverName,
                TransactionDate = medicineInventory.TransactionDate,
                SupplierId = medicineInventory.SupplierId,
                SupplierName = medicineInventory.SupplierName, // Kiểm tra null trước khi lấy giá trị
                ManufacturingDate = medicineInventory.ManufacturingDate,
                BatchNumber = medicineInventory.BatchNumber,
            }).ToList();

            return medicineInventoryDTOs;
        }



        //Get one medical supply
        public MedicineDTO? GetMedicineById(int medicineId)
        {
            var medicine = _medicineRepository.GetMedicine(medicineId);
            if (medicine == null)
                return null;
            var medicineDTO = new MedicineDTO
            {
                MedicineId = medicine.MedicineId,
                MedicineName = medicine.MedicineName,
                ActiveIngredient = medicine.ActiveIngredient,
                Dosage = medicine.Dosage,
                DosageForm = medicine.DosageForm,
                ImportPrice = medicine.ImportPrice,
                SellingPrice = medicine.SellingPrice,
                Quantity = _medicineRepository.GetMedicineQuantity(medicine.MedicineId),
                ShelfLife = medicine.ShelfLife,
                //BatchNumber = medicine.MedicineInventories.FirstOrDefault()?.BatchNumber,
                BidNumber = medicine.BidNumber,
                //SupplierName = medicine.MedicineInventories.FirstOrDefault()?.Supplier?.Name,
                //ManufacturingDate = medicine.MedicineInventories.FirstOrDefault()?.ManufacturingDate,
                //ExpiryDate = medicine.MedicineInventories.FirstOrDefault()?.ExpiryDate,
                Status = medicine.Status
            };
            return medicineDTO;
        }

        //Get medicine supply detail
        public List<MedicineInventoryDTO> MedicineDetail(int medicineId)
        {
            List<MedicineInventoryDTO> medicineInventoryDTOs = new List<MedicineInventoryDTO>();
            List<MedicineInventory> medicineInventories = _medicineRepository.GetMedicineInventory(medicineId);
            foreach (var medicineInventory in medicineInventories)
            {
                var medicineInventoryDTO = new MedicineInventoryDTO
                {
                    MedicineInventoryId = medicineInventory.MedicineInventoryId,
                    MedicineId = medicineInventory.MedicineId,
                    Quantity = medicineInventory.Quantity,
                    CertificateNumber = medicineInventory.CertificateNumber,
                    ManufacturingDate = medicineInventory.ManufacturingDate,
                    TransactionDate = medicineInventory.TransactionDate,
                    ExpiryDate = medicineInventory.ExpiryDate,
                    Note = medicineInventory.Note,
                    ReceiverId = medicineInventory.ReceiverId,
                    TransactionType = medicineInventory.TransactionType,
                    BatchNumber = medicineInventory.BatchNumber,
                    //BidNumber = medicineInventory.Medicine.BidNumber,
                };
                medicineInventoryDTOs.Add(medicineInventoryDTO);
            }
            return medicineInventoryDTOs;
        }

        //search medicine by name
        public List<MedicineDTO> SearchMedicineByName(string name)
        {
            List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();
            var medicines = _medicineRepository.SearchMedicineByName(name);
            foreach (var medicine in medicines)
            {
                var medicineDTO = new MedicineDTO
                {
                    MedicineId = medicine.MedicineId,
                    MedicineName = medicine.MedicineName,
                    ActiveIngredient = medicine.ActiveIngredient,
                    Dosage = medicine.Dosage,
                    IsBhyt = medicine.IsBhyt,
                    ExpiryDate = medicine.MedicineInventories.FirstOrDefault()?.ExpiryDate,
                    ManufacturingDate = medicine.MedicineInventories.FirstOrDefault()?.ManufacturingDate,
                    DosageForm = medicine.DosageForm,
                    ImportPrice = medicine.ImportPrice,
                    SellingPrice = medicine.SellingPrice,
                    Quantity = _medicineRepository.GetMedicineQuantity(medicine.MedicineId),
                    ShelfLife = medicine.ShelfLife,
                    BidNumber = medicine.BidNumber,
                    Status = medicine.Status
                };
                medicineDTOs.Add(medicineDTO);
            }
            return medicineDTOs;
        }

        public async Task<List<MedicineDTO>> SearchMedicinesAsync(
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
            bool? status = null,
            DateTime? minExpiryDate = null,
            DateTime? maxExpiryDate = null)
        {
            // Gọi repository để lấy dữ liệu
            var medicines = await _medicineRepository.SearchMedicinesAsync(
                medicineId, medicineName, activeIngredient, dosage, dosageForm, quantity,
                importPrice, expiryDate, batchNumber, bidNumber, status, minExpiryDate, maxExpiryDate
            );

            if (medicines == null || medicines.Count == 0)
            {
                return new List<MedicineDTO>(); // Trả về danh sách DTO rỗng
            }

            // Chuyển đổi dữ liệu thành DTO
            var result = medicines.Select(m =>
            {
                var validInventories = m.MedicineInventories
                    .Where(mi => mi.ExpiryDate.HasValue)
                    .OrderByDescending(mi => mi.ExpiryDate)
                    .ToList();

                return new MedicineDTO
                {
                    MedicineId = m.MedicineId,
                    MedicineName = m.MedicineName,
                    ActiveIngredient = m.ActiveIngredient,
                    Dosage = m.Dosage,
                    DosageForm = m.DosageForm,
                    ImportPrice = m.ImportPrice,
                    SellingPrice = m.SellingPrice,
                    ShelfLife = m.ShelfLife,
                    BidNumber = m.BidNumber,
                    Status = m.Status,
                    IsBhyt = m.IsBhyt,
                    ExpiryDate = validInventories.Select(mi => mi.ExpiryDate).FirstOrDefault(),
                    BatchNumber = validInventories.Select(mi => mi.BatchNumber).FirstOrDefault(),
                    Quantity = validInventories.Select(mi => mi.Quantity).FirstOrDefault()
                };
            }).ToList();

            return result;
        }


        public bool AddMedicineInventory(MedicineInventoryDTO medicineInventoryDTO)
        {
            var medicineData = _medicineRepository.GetMedicine(medicineInventoryDTO.MedicineId);
            int? shelfLife = medicineData?.ShelfLife;

            var expiryDate = _medicineRepository.CalculateExpiryDate(
                medicineInventoryDTO.ManufacturingDate,
                shelfLife
            );
            var medicine = new MedicineInventory
            {
                MedicineId = medicineInventoryDTO.MedicineId,
                Quantity = medicineInventoryDTO.Quantity,
                CertificateNumber = medicineInventoryDTO.CertificateNumber,
                ManufacturingDate = medicineInventoryDTO.ManufacturingDate,
                TransactionDate = medicineInventoryDTO.TransactionDate,
                ExpiryDate = expiryDate,
                Note = medicineInventoryDTO.Note,
                ReceiverId = medicineInventoryDTO.ReceiverId,
                TransactionType = medicineInventoryDTO.TransactionType,
                BatchNumber = medicineInventoryDTO.BatchNumber,
                SupplierId = medicineInventoryDTO.SupplierId,
            };
            return _medicineRepository.AddMedicineInventory(medicine);
        }

        public bool UpdateMedicineInventory(MedicineInventoryDTO medicineInventoryDTO)
        {
            var medicineData = _medicineRepository.GetMedicine(medicineInventoryDTO.MedicineId);
            int? shelfLife = medicineData?.ShelfLife;

            var expiryDate = _medicineRepository.CalculateExpiryDate(
                medicineInventoryDTO.ManufacturingDate,
                shelfLife
            );
            var medicineInventory = new MedicineInventory
            {
                MedicineInventoryId = medicineInventoryDTO.MedicineInventoryId,
                MedicineId = medicineInventoryDTO.MedicineId,
                Quantity = medicineInventoryDTO.Quantity,
                CertificateNumber = medicineInventoryDTO.CertificateNumber,
                ManufacturingDate = medicineInventoryDTO.ManufacturingDate,
                TransactionDate = medicineInventoryDTO.TransactionDate,
                ExpiryDate = expiryDate,
                Note = medicineInventoryDTO.Note,
                ReceiverId = medicineInventoryDTO.ReceiverId,
                TransactionType = medicineInventoryDTO.TransactionType,
                BatchNumber = medicineInventoryDTO.BatchNumber,
                SupplierId = medicineInventoryDTO.SupplierId,
            };
            if (!_medicineRepository.UpdateMedicineInventory(medicineInventory)) return false;
            return true;
            //return _medicineRepository.UpdateMedicineInventory(medicineInventory);
        }

        public async Task<List<MedicineSuggestionDTO>> GetMedicineSuggestions(string query)
        {
            // Lấy dữ liệu thuốc từ repository (Google Sheets)
            var data = await _medicineRepository.SearchMedicinesData(query);

            var suggestions = new List<MedicineSuggestionDTO>();

            // Lọc và ánh xạ dữ liệu vào DTO
            foreach (var row in data.table.rows)
            {
                var medicineName = GetValueFromJToken(row.c[1]);

                // Skip header row and empty rows
                if (string.IsNullOrEmpty(medicineName) || medicineName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Chỉ lấy những thuốc bắt đầu bằng từ khóa tìm kiếm (không phân biệt hoa thường)
                if (string.IsNullOrEmpty(query) || medicineName.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                {
                    var suggestion = new MedicineSuggestionDTO
                    {
                        MedicineName = medicineName,
                        ActiveIngredient = GetValueFromJToken(row.c[2]),
                        Dosage = GetValueFromJToken(row.c[3]),
                        DosageForm = GetValueFromJToken(row.c[6]),
                        UnitPrice = GetValueFromJToken(row.c[12]),
                        ShelfLife = GetValueFromJToken(row.c[14])
                    };
                    suggestions.Add(suggestion);
                }
            }

            // Sắp xếp kết quả theo tên thuốc để trả về danh sách có tổ chức
            return suggestions.OrderBy(s => s.MedicineName).ToList();
        }

        private string GetValueFromJToken(JToken token)
        {
            if (token == null)
                return "";

            // If the token is a JValue
            if (token is JValue jValue)
                return jValue.Value?.ToString() ?? "";

            // If the token is a JObject with 'v' property
            if (token["v"] != null)
                return token["v"].ToString();

            // As a last resort, try direct ToString()
            return token.ToString() ?? "";
        }
    }
}