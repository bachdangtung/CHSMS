using CHSMS.API.DTOs;
using CHSMS.API.DTOs.Medicine;
using CHSMS.API.DTOs.Medicine;
using CHSMS.API.DTOs.MedicineInventory;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;

namespace CHSMS.API.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly SEP_TestContext _context;
        private readonly IMedicineRepository _medicineRepository;
        private readonly ILogger<MedicineService> _logger;

        public MedicineService(IMedicineRepository medicineRepository, SEP_TestContext context, ILogger<MedicineService> logger)
        {
            _medicineRepository = medicineRepository;
            _context = context;
            _logger = logger;
        }

        public List<MedicineInventoryGetAllDTO> GetAllMedicineInInventory()
        {
            var inventories = _medicineRepository.GetAllMedicineInventories();

            var dtoList = inventories.Select(inventory => new MedicineInventoryGetAllDTO
            {
                MedicineId = inventory.MedicineId,
                MedicineName = inventory.Medicine?.MedicineName ?? "Không rõ",
                ActiveIngredient = inventory.Medicine?.ActiveIngredient ?? "Không rõ",
                Dosage = inventory.Medicine?.Dosage ?? "Không rõ",
                DosageForm = inventory.Medicine?.DosageForm ?? "Không rõ",
                ImportPrice = inventory.Medicine?.ImportPrice ?? 0,
                BidNumber = inventory.Medicine?.BidNumber ?? "Không rõ",
                BatchNumber = inventory.BatchNumber,
                Quantity = inventory.Quantity,
                ManufacturingDate = inventory.ManufacturingDate,
                ExpiryDate = inventory.ExpiryDate,
                IsBhyt = inventory.Medicine?.IsBhyt ?? false,
                Status = inventory.Medicine?.Status ?? false,
            }).ToList();

            return dtoList;
        }


        // Get all medicines
        public List<MedicineDTO> GetAllMedicine()
        {
            List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();
            foreach (var medicine in _medicineRepository.GetAllMedicine())
            {
                double quantity = _medicineRepository.GetMedicineQuantity(medicine.MedicineId);
                if (quantity < 0)
                    continue;
                var medicineDTO = ConvertToMedicineDTO(medicine);
                medicineDTO.Quantity = quantity;
                medicineDTOs.Add(medicineDTO);
            }
            return medicineDTOs;
        }

        public List<MedicineDTO> GetAllActualMedicines(DateTime? date)
        {
            if (date == null)
            {
                return GetAllMedicine();
            }
            List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();
            var medicalSupplies = _medicineRepository.GetAllMedicine();
            foreach (var item in medicalSupplies)
            {
                var medicineDTO = ConvertToMedicineDTO(item);
                medicineDTO.Quantity = _medicineRepository.GetActualMedicineQuantity(item.MedicineId, date.Value);
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
        public List<MedicineInventoryDetailDTO> GetMedicineInventoryByMedicineId(int medicineId)
        {
            var medicineInventories = _medicineRepository.GetMedicineInventoryByMedicineId(medicineId);

            var medicineInventoryDTOs = medicineInventories.Select(medicineInventory => new MedicineInventoryDetailDTO
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
                SupplierName = medicineInventory.SupplierName,
                ManufacturingDate = medicineInventory.ManufacturingDate,
                BatchNumber = medicineInventory.BatchNumber,
            }).ToList();

            return medicineInventoryDTOs;
        }

        public Medicine GetMedicineByMedicineInventoryId(int medicineInventoryId)
        {
            return _medicineRepository.GetMedicineByMedicineInventoryId(medicineInventoryId);
        }

        public MedicineInventory GetMedicineInventoryById(int? medicineInventoryId)
        {
            return _medicineRepository.GetMedicineInventoryById(medicineInventoryId.Value);
        }

        public bool UpdateMedicineConsumption(ConsumeMedicineDTO medicineConsumption)
        {
            if (medicineConsumption == null)
            {
                return false;
            }
            var MSC = _medicineRepository.GetMedicineConsumptionById(medicineConsumption.ConsumeMedicineId.Value);
            if (MSC == null)
            {
                return false;
            }
            var medicineInventory = _medicineRepository.GetMedicineInventoryById(medicineConsumption.MedicineInventoryId.Value);
            if (medicineInventory == null)
            {
                return false;
            }
            var numberUpdate = medicineConsumption.Quantity.Value - MSC.Amount.Value;
            medicineInventory.Quantity -= numberUpdate;
            if (medicineInventory.Quantity < 0)
            {
                return false;
            }
            var result1 = _medicineRepository.UpdateMedicineInventory(medicineInventory);
            MSC.Amount = medicineConsumption.Quantity;
            MSC.Status = medicineConsumption.Status;
            MSC.Note = medicineConsumption.Note;
            var result = _medicineRepository.UpdateMedicineConsumption(MSC);

            if (result1 && result)
            {
                return true;
            }
            return false;
        }
        public List<MedicineInventory>? GetMedicineImportHistory(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate || fromDate > DateTime.Now)
            {
                return null;
            }
            return _medicineRepository.GetMedicineImportHistory(fromDate, toDate);
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
        public List<MedicineInventoryDetailDTO> MedicineDetail(int medicineId)
        {
            List<MedicineInventoryDetailDTO> medicineInventoryDTOs = new List<MedicineInventoryDetailDTO>();
            List<MedicineInventory> medicineInventories = _medicineRepository.GetMedicineInventory(medicineId);
            foreach (var medicineInventory in medicineInventories)
            {
                var medicineInventoryDTO = new MedicineInventoryDetailDTO
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
            var inventories = await _medicineRepository.SearchMedicinesAsync(
                medicineId, medicineName, activeIngredient, dosage, dosageForm,
                quantity, importPrice, expiryDate, batchNumber, bidNumber, status,
                minExpiryDate, maxExpiryDate
            );

            if (inventories == null || !inventories.Any())
                return new List<MedicineDTO>();

            var result = inventories.Select(mi => new MedicineDTO
            {
                MedicineId = mi.MedicineId,
                MedicineName = mi.MedicineName,
                ActiveIngredient = mi.ActiveIngredient,
                Dosage = mi.Dosage,
                DosageForm = mi.DosageForm,
                ImportPrice = mi.ImportPrice,
                SellingPrice = mi.SellingPrice,
                ShelfLife = mi.ShelfLife,
                BidNumber = mi.BidNumber,
                Status = mi.Status,
                IsBhyt = mi.IsBhyt,
                /*ExpiryDate = mi.MedicineInventories.ExpiryDate,
                BatchNumber = mi.BatchNumber,
                Quantity = mi.Quantity ?? 0*/
            }).ToList();

            return result;
        }




        public AddMedicineInventoryResultDTO AddMedicineInventoryList(List<MedicineInventoryAddDTO> dtoList, int userId)
        {
            var result = new AddMedicineInventoryResultDTO();
            var inventoryList = new List<MedicineInventory>();

            if (dtoList == null || !dtoList.Any())
            {
                _logger.LogWarning("Danh sách DTO trống khi thêm thuốc.");
                result.Warnings.Add("Danh sách DTO trống.");
                return result;
            }

            foreach (var dto in dtoList)
            {
                bool isDuplicate = _medicineRepository.CheckDuplicateBatch(
                    dto.MedicineId,
                    dto.BatchNumber,
                    dto.TransactionDate
                );

                if (isDuplicate)
                {
                    var warning = $"Thuốc ID {dto.MedicineId} với số lô {dto.BatchNumber} đã nhập trong ngày {dto.TransactionDate.ToString("dd/MM/yyyy")}";
                    _logger.LogWarning(warning);
                    result.Warnings.Add(warning);
                    continue;
                }

                var medicineData = _medicineRepository.GetMedicine(dto.MedicineId);
                if (medicineData == null)
                {
                    var warning = $"Thuốc ID {dto.MedicineId} không tồn tại.";
                    _logger.LogWarning(warning);
                    result.Warnings.Add(warning);
                    continue;
                }

                if (dto.SupplierId <= 0)
                {
                    var warning = $"Nhà cung cấp không hợp lệ cho thuốc ID {dto.MedicineId}.";
                    _logger.LogWarning(warning);
                    result.Warnings.Add(warning);
                    continue;
                }

                int? shelfLife = medicineData.ShelfLife;
                var expiryDate = _medicineRepository.CalculateExpiryDate(dto.ManufacturingDate, shelfLife);

                var medicine = new MedicineInventory
                {
                    MedicineId = dto.MedicineId,
                    Quantity = dto.ImportQuantity,
                    ImportQuantity = dto.ImportQuantity,
                    CertificateNumber = dto.CertificateNumber ?? string.Empty,
                    ManufacturingDate = dto.ManufacturingDate,
                    TransactionDate = dto.TransactionDate,
                    ExpiryDate = expiryDate,
                    Note = dto.Note ?? string.Empty,
                    ReceiverId = userId,
                    TransactionType = dto.TransactionType,
                    BatchNumber = dto.BatchNumber,
                    SupplierId = dto.SupplierId
                };

                inventoryList.Add(medicine);
            }

            if (inventoryList.Any())
            {
                try
                {
                    bool saved = _medicineRepository.AddMedicineInventoryList(inventoryList);
                    result.IsSuccess = saved;
                    result.AddedCount = saved ? inventoryList.Count : 0;
                    if (!saved)
                    {
                        _logger.LogError("Lỗi khi lưu danh sách thuốc vào database.");
                        result.Warnings.Add("Lỗi khi lưu dữ liệu vào database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lưu danh sách thuốc.");
                    result.Warnings.Add("Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Không có thuốc nào được thêm do lỗi dữ liệu.");
                result.Warnings.Add("Không có thuốc nào được thêm do lỗi dữ liệu.");
            }

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
            /*existing.MedicineId = dto.MedicineId;
            existing.CertificateNumber = dto.CertificateNumber;
            existing.SupplierId = dto.SupplierId;
            existing.TransactionType = dto.TransactionType;*/
            existing.ImportQuantity = dto.ImportQuantity;
            existing.Quantity = dto.Quantity;
            existing.Note = dto.Note;
            /*existing.BatchNumber = dto.BatchNumber;*/
            existing.ManufacturingDate = dto.ManufacturingDate;
            /*            existing.TransactionDate = dto.TransactionDate;
            */
            return _medicineRepository.SaveChanges();
        }

        public int ConsumeMedicine(ConsumeMedicineDTO consumeMedicineDTO)
        {
            return _medicineRepository.ConsumeMedicineByMedicineId(consumeMedicineDTO);
        }

        public Dictionary<MedicineDTO, double> ConsumeReport(DateTime? from, DateTime? to)
        {
            MedicineDTO medicineDTO;
            Dictionary<MedicineDTO, double> result = new Dictionary<MedicineDTO, double>();
            var dict = _medicineRepository.GetAllMedicineConsumeReport(from, to);
            foreach (var item in dict)
            {
                medicineDTO = new MedicineDTO();
                medicineDTO = ConvertToMedicineDTO(item.Key);
                medicineDTO.Quantity = _medicineRepository.GetMedicineQuantityById(medicineDTO.MedicineId);
                result.Add(medicineDTO, item.Value);
            }
            return result;
        }

        public double GetAddOnMedicineInventory(int id, DateTime? from, DateTime? to)
        {
            return _medicineRepository.GetAddOnMedicineInventory(id, from, to);
        }

        public object GetExpiryMedicineInventory(int medicineId, DateTime? from, DateTime? to)
        {
            return _medicineRepository.GetNumberOfExpiredMedicineInventory(medicineId, from, to);
        }

        public List<MedicineConsumption> ConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _medicineRepository.MedicineConsumptionDetail(id, from, to);
            return result;
        }
        public List<MedicineConsumption> ConsumptionHistory(DateTime? from, DateTime? to)
        {
            return _medicineRepository.ConsumptionHistory(from, to);
        }
        public List<MedicineInventory> GetRecentInventoryHistory(int userId)
        {
            return _medicineRepository.GetRecentInventoriesByUser(userId);
        }
        public List<MedicineInventoryUpdateHistoryDTO> GetAllInventoryHistory(int userId)
        {
            var inventories = _medicineRepository.GetAllInventoriesByUser(userId);
            return inventories.Select(x =>
            {
                bool isWithin24Hours = x.TransactionDate >= DateTime.Now.AddHours(-24);

                return new MedicineInventoryUpdateHistoryDTO
                {
                    MedicineInventoryId = x.MedicineInventoryId,
                    MedicineId = x.MedicineId,
                    CertificateNumber = x.CertificateNumber,
                    ManufacturingDate = x.ManufacturingDate,
                    ExpiryDate = x.ExpiryDate,
                    TransactionType = x.TransactionType,
                    BatchNumber = x.BatchNumber,
                    SupplierId = x.SupplierId,
                    Note = x.Note,
                    ImportQuantity = x.ImportQuantity,
                    Quantity = x.Quantity,
                    TransactionDate = x.TransactionDate,

                    // Thiết lập quyền chỉnh sửa chung
                    CanEdit = isWithin24Hours,

                    // Thiết lập quyền chỉnh sửa cho các trường cụ thể
                    CanEditNote = isWithin24Hours,
                    CanEditImportQuantity = isWithin24Hours,
                    CanEditManufacturingDate = isWithin24Hours
                };
            }).ToList();
        }


        public List<MedicineDTO> FilterMedicineStock(MedicineInventoryFilter filter)
        {
            return _medicineRepository.GetFilteredMedicineInventory(filter);
        }

        //convert to MedicineDTO
        private MedicineDTO ConvertToMedicineDTO(Medicine medicine)
        {
            return new MedicineDTO
            {
                MedicineId = medicine.MedicineId,
                MedicineName = medicine.MedicineName,
                MedicineCode = medicine.MedicineCode,
                ActiveIngredient = medicine.ActiveIngredient,
                Dosage = medicine.Dosage,
                DosageForm = medicine.DosageForm,
                ImportPrice = medicine.ImportPrice,
                SellingPrice = medicine.SellingPrice,
                ShelfLife = medicine.ShelfLife,
                BatchNumber = medicine.MedicineInventories.FirstOrDefault()?.BatchNumber,
                BidNumber = medicine.BidNumber,
                IsBhyt = medicine.IsBhyt,
                ManufacturingDate = medicine.MedicineInventories.FirstOrDefault()?.ManufacturingDate,
                ExpiryDate = medicine.MedicineInventories.FirstOrDefault()?.ExpiryDate,
                Status = medicine.Status
            };
        }
    }
}
