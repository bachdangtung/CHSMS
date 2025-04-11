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
                    MedicineCode = medicine.MedicineCode,
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
                ImportQuantity = medicineInventory.ImportQuantity,
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
            // Gọi repository nhưng **không truyền tham số phân trang**
            var medicines = await _medicineRepository.SearchMedicinesAsync(
                medicineId, medicineName, activeIngredient, dosage, dosageForm, quantity,
                importPrice, expiryDate, batchNumber, bidNumber, status, minExpiryDate, maxExpiryDate
            );

            if (medicines == null || !medicines.Any())
            {
                return new List<MedicineDTO>();
            }

            // Chuyển đổi entity thành DTO
            var result = medicines.Select(m =>
            {
                var validInventories = m.MedicineInventories
                    .Where(mi => mi.ExpiryDate.HasValue && mi.Quantity > 0)
                    .OrderBy(mi => mi.ExpiryDate)
                    .ToList();

                double totalQuantity = validInventories.Sum(mi => mi.Quantity ?? 0);

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
                    ExpiryDate = validInventories.FirstOrDefault()?.ExpiryDate,
                    BatchNumber = validInventories.FirstOrDefault()?.BatchNumber,
                    Quantity = totalQuantity
                };
            }).ToList();

            return result;
        }


        public AddMedicineInventoryResultDTO AddMedicineInventoryList(List<MedicineInventoryAddDTO> dtoList, int userId)
        {
            var result = new AddMedicineInventoryResultDTO();
            var inventoryList = new List<MedicineInventory>();

            foreach (var dto in dtoList)
            {
                bool isDuplicate = _medicineRepository.CheckDuplicateBatch(
                    dto.MedicineId,
                    dto.BatchNumber,
                    dto.TransactionDate
                );

                if (isDuplicate)
                {
                    result.Warnings.Add($"Thuốc ID {dto.MedicineId} với số lô {dto.BatchNumber} đã nhập trong ngày {dto.TransactionDate?.ToString("dd/MM/yyyy")}");
                }

                var medicineData = _medicineRepository.GetMedicine(dto.MedicineId);
                int? shelfLife = medicineData?.ShelfLife;

                var expiryDate = _medicineRepository.CalculateExpiryDate(dto.ManufacturingDate, shelfLife);

                var medicine = new MedicineInventory
                {
                    MedicineId = dto.MedicineId,
                    Quantity = dto.Quantity,
                    CertificateNumber = dto.CertificateNumber,
                    ManufacturingDate = dto.ManufacturingDate,
                    TransactionDate = dto.TransactionDate,
                    ExpiryDate = expiryDate,
                    Note = dto.Note,
                    ReceiverId = userId,
                    TransactionType = dto.TransactionType,
                    BatchNumber = dto.BatchNumber,
                    SupplierId = dto.SupplierId,
                };

                inventoryList.Add(medicine);
            }

            // Gọi hàm AddMedicineInventoryBatch trong repo
            bool saved = _medicineRepository.AddMedicineInventoryList(inventoryList);
            result.IsSuccess = saved;
            result.AddedCount = saved ? inventoryList.Count : 0;

            return result;
        }

        public bool UpdateMedicineInventory(MedicineInventoryUpdateDTO dto, int userId)
        {
            var existing = _medicineRepository.GetInventoryById(dto.MedicineInventoryId);
            if (existing == null)
                throw new Exception("Không tìm thấy bản ghi.");

            if (existing.ReceiverId != userId)
                throw new Exception("Bạn không có quyền sửa bản ghi này.");

            if ((DateTime.Now - existing.TransactionDate)?.TotalHours > 24)
                throw new Exception("Bản ghi đã quá 24 giờ, không thể chỉnh sửa.");

            // cập nhật các trường được phép
            existing.MedicineId = dto.MedicineId;
            existing.CertificateNumber = dto.CertificateNumber;
            existing.SupplierId = dto.SupplierId;
            existing.TransactionType = dto.TransactionType;
            existing.ImportQuantity = dto.ImportQuantity;
            existing.Quantity = dto.Quantity;
            existing.Note = dto.Note;
            existing.BatchNumber = dto.BatchNumber;
            existing.ManufacturingDate = dto.ManufacturingDate;
            existing.TransactionDate = dto.TransactionDate;

            return _medicineRepository.SaveChanges();
        }
        public List<MedicineInventory> GetRecentInventoryHistory(int userId)
        {
            return _medicineRepository.GetRecentInventoriesByUser(userId);
        }

        public List<MedicineDTO> FilterMedicineStock(MedicineInventoryFilter filter)
        {
            return _medicineRepository.GetFilteredMedicineInventory(filter);
        }
    }
}
