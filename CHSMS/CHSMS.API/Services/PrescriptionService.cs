using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.DTOs.MedicineInventory;


public class PrescriptionService
{
    private readonly PrescriptionRepository _repository;
    
    private readonly SEP_TestContext _context;
    

    public PrescriptionService( PrescriptionRepository repository,SEP_TestContext context)

    {
        _repository = repository;
        _context = context;
        
    }

    public async Task<int> CreatePrescriptionAsync(CreatePrescriptionDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Business Rule 3: Kiểm tra IssueDate
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Business Rule 4: Kiểm tra số lượng tối đa loại thuốc (dựa trên MedicineId)
            var medicineIds = new List<int>();
            foreach (var medDto in dto.MedicineConsumptions)
            {
                var inventory = await _repository.GetMedicineInventoryByIdAsync(medDto.MedicineInventoryId);
                if (inventory == null)
                    throw new Exception($"Không tìm thấy kho thuốc với ID: {medDto.MedicineInventoryId}");
                medicineIds.Add(inventory.MedicineId);
            }
            if (medicineIds.Distinct().Count() > 10)
                throw new Exception("Một đơn thuốc không được chứa quá 10 loại thuốc!");

            // Kiểm tra trùng MedicineInventoryId
            var medicineInventoryIds = dto.MedicineConsumptions.Select(mc => mc.MedicineInventoryId).ToList();
            if (medicineInventoryIds.Distinct().Count() != medicineInventoryIds.Count)
                throw new Exception("Có thuốc bị trùng trong đơn thuốc. Vui lòng kiểm tra lại!");

            // Tạo Prescription với Status mặc định là true
            var prescription = new Prescription
            {
                MedicalRecordHistoryId = dto.MedicalRecordHistoryId,
                UserId = dto.UserId,
                IssueDate = dto.IssueDate,
                Status = true, // Mặc định là true
                Note = dto.Note,
                IsBhyt = dto.IsBhyt
            };
            var createdPrescription = await _repository.CreatePrescriptionAsync(prescription);

            // Tạo MedicineConsumption
            foreach (var medDto in dto.MedicineConsumptions)
            {
                var inventory = await _repository.GetMedicineInventoryByIdAsync(medDto.MedicineInventoryId);
                if (inventory == null)
                    throw new Exception($"Không tìm thấy kho thuốc với ID: {medDto.MedicineInventoryId}");

                // Kiểm tra số lượng và hạn sử dụng
                if (medDto.Amount > (inventory.Quantity ?? 0))
                    throw new Exception($"Số lượng yêu cầu vượt quá tồn kho");
                if (medDto.ConsumptionDate > (inventory.ExpiryDate ?? DateTime.MaxValue) || dto.IssueDate > (inventory.ExpiryDate ?? DateTime.MaxValue))
                    throw new Exception($"Ngày sử dụng vượt quá hạn sử dụng");

                // Business Rule 8: Kiểm tra số lượng tồn kho tối thiểu (cho bác sĩ)
                const int minimumQuantity = 10;
                if ((inventory.Quantity ?? 0) - medDto.Amount < minimumQuantity)
                {
                    throw new Exception($"Số lượng tồn kho của thuốc ID {medDto.MedicineInventoryId} sẽ dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi tạo đơn thuốc!");
                }

                var consumption = new MedicineConsumption
                {
                    MedicineInventoryId = medDto.MedicineInventoryId,
                    Amount = medDto.Amount,
                    ConsumptionDate = medDto.ConsumptionDate,
                    IsSpecialMedicine = medDto.IsSpecialMedicine,
                    Note = medDto.Note,
                    Status = false // Mặc định là false
                };
                var createdConsumption = await _repository.CreateMedicineConsumptionAsync(consumption);

                var pmc = new PrescriptionMedicineConsumption
                {
                    PrescriptionId = createdPrescription.PrescriptionId,
                    MedicineConsumtionId = createdConsumption.MedicineConsumptionId,
                    TotalPrice = 0 // Chưa tính TotalPrice
                };
                await _repository.CreatePrescriptionMedicineConsumptionAsync(pmc);
            }

            await transaction.CommitAsync();
            return createdPrescription.PrescriptionId;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Lỗi khi tạo đơn thuốc: {ex.Message}");
        }
    }

    public async Task EditPrescriptionForDoctorAsync(EditPrescriptionForDoctorDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Kiểm tra ngày phát hành không được trong tương lai
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Kiểm tra trạng thái đơn thuốc
            var prescription = await _repository.GetPrescriptionByIdAsync(dto.PrescriptionId);
            if (prescription == null)
                throw new Exception($"Không tìm thấy đơn thuốc với ID: {dto.PrescriptionId}");

            
            var consumptions = await _repository.GetMedicineConsumptionsByPrescriptionIdAsync(dto.PrescriptionId);
            if (consumptions.All(c => c.Status ?? false))
                throw new Exception("Đơn thuốc đã được xác nhận hoàn tất, không thể chỉnh sửa!");

            
            prescription.MedicalRecordHistoryId = dto.MedicalRecordHistoryId;
            prescription.UserId = dto.UserId;
            prescription.IssueDate = dto.IssueDate;
            prescription.Note = dto.Note;
            prescription.IsBhyt = dto.IsBhyt;
            await _repository.UpdatePrescriptionAsync(prescription);

            
            foreach (var consumptionId in dto.MedicineConsumptionIdsToRemove)
            {
                var pmc = await _repository.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(consumptionId);
                if (pmc != null)
                {
                    await _repository.DeletePrescriptionMedicineConsumptionAsync(pmc.PrescriptionId, pmc.MedicineConsumtionId);
                    await _repository.DeleteMedicineConsumptionAsync(consumptionId);
                }
            }

            // Số lượng thuốc có trong 1 đơn
            var existingConsumptions = await _repository.GetMedicineConsumptionsByPrescriptionIdAsync(dto.PrescriptionId);
            if (existingConsumptions.Count() + dto.MedicineConsumptionsToAdd.Count > 10)
                throw new Exception("Một đơn thuốc không được chứa quá 10 loại thuốc!");

            // Kiểm tra thuốc bị trùng
            var medicineInventoryIds = dto.MedicineConsumptionsToAdd.Select(mc => mc.MedicineInventoryId).ToList();
            if (medicineInventoryIds.Distinct().Count() != medicineInventoryIds.Count)
                throw new Exception("Có thuốc bị trùng trong danh sách thêm mới. Vui lòng kiểm tra lại!");

            
            foreach (var medDto in dto.MedicineConsumptionsToAdd)
            {
                var inventory = await _repository.GetMedicineInventoryByIdAsync(medDto.MedicineInventoryId);
                if (inventory == null)
                    throw new Exception($"Không tìm thấy kho thuốc với ID: {medDto.MedicineInventoryId}");

                if (medDto.Amount > (inventory.Quantity ?? 0))
                    throw new Exception($"Số lượng yêu cầu vượt quá tồn kho");
                if (medDto.ConsumptionDate > (inventory.ExpiryDate ?? DateTime.MaxValue) || dto.IssueDate > (inventory.ExpiryDate ?? DateTime.MaxValue))
                    throw new Exception($"Ngày sử dụng vượt quá hạn sử dụng");

                // Business Rule 8: Kiểm tra số lượng tồn kho tối thiểu (cho bác sĩ)
                const int minimumQuantity = 10;
                if ((inventory.Quantity ?? 0) - medDto.Amount < minimumQuantity)
                {
                    throw new Exception($"Số lượng tồn kho của thuốc ID {medDto.MedicineInventoryId} sẽ dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi thêm vào đơn thuốc!");
                }

                var consumption = new MedicineConsumption
                {
                    MedicineInventoryId = medDto.MedicineInventoryId,
                    Amount = medDto.Amount,
                    ConsumptionDate = medDto.ConsumptionDate,
                    IsSpecialMedicine = medDto.IsSpecialMedicine,
                    Note = medDto.Note,
                    Status = false // Mặc định là false
                };
                var createdConsumption = await _repository.CreateMedicineConsumptionAsync(consumption);

                var pmc = new PrescriptionMedicineConsumption
                {
                    PrescriptionId = dto.PrescriptionId,
                    MedicineConsumtionId = createdConsumption.MedicineConsumptionId,
                    TotalPrice = 0 // Chưa tính TotalPrice
                };
                await _repository.CreatePrescriptionMedicineConsumptionAsync(pmc);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Lỗi khi chỉnh sửa đơn thuốc: {ex.Message}");
        }
    }

    public async Task EditPrescriptionForPharmacistAsync(EditPrescriptionForPharmacistDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Lấy Prescription
            var prescription = await _repository.GetPrescriptionByIdAsync(dto.PrescriptionId);
            if (prescription == null)
                throw new Exception($"Không tìm thấy đơn thuốc với ID: {dto.PrescriptionId}");

            // Business Rule 9: Kiểm tra thời gian hiệu lực của đơn thuốc
            var expiryDays = 7; // Đơn thuốc có hiệu lực trong 7 ngày
            if (!prescription.IssueDate.HasValue || (DateTime.Now - prescription.IssueDate.Value).TotalDays > expiryDays)
                throw new Exception("Đơn thuốc đã hết hạn hoặc không có ngày phát hành, không thể chỉnh sửa trạng thái!");

            foreach (var statusDto in dto.MedicineConsumptionStatuses)
            {
                var consumption = await _repository.GetMedicineConsumptionByIdAsync(statusDto.MedicineConsumptionId);
                if (consumption == null)
                    throw new Exception($"Không tìm thấy MedicineConsumption với ID: {statusDto.MedicineConsumptionId}");

                // Cập nhật Status
                consumption.Status = statusDto.Status;
                await _repository.UpdateMedicineConsumptionAsync(consumption);

                // Nếu Status được đổi thành true, tính TotalPrice và trừ Quantity
                if (statusDto.Status)
                {
                    if (!consumption.MedicineInventoryId.HasValue)
                    {
                        throw new Exception($"MedicineInventoryId không được để trống trong MedicineConsumption với ID: {consumption.MedicineConsumptionId}");
                    }
                    var inventory = await _repository.GetMedicineInventoryByIdAsync(consumption.MedicineInventoryId.Value);
                    if (inventory == null)
                        throw new Exception($"Không tìm thấy kho thuốc với ID: {consumption.MedicineInventoryId}");

                    // Trừ Quantity
                    inventory.Quantity -= (consumption.Amount ?? 0);
                    if (inventory.Quantity < 0)
                        throw new Exception($"Số lượng tồn kho không đủ để phát thuốc!");

                    // Kiểm tra số lượng tồn kho tối thiểu (cho dược sĩ)
                    const int minimumQuantity = 10;
                    if (inventory.Quantity < minimumQuantity)
                    {
                        throw new Exception($"Số lượng tồn kho của thuốc ID {consumption.MedicineInventoryId} dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi phát thuốc!");
                    }

                    await _repository.UpdateMedicineInventoryAsync(inventory);

                    // Tính TotalPrice
                    var pmc = await _repository.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(consumption.MedicineConsumptionId);
                    pmc.TotalPrice = (consumption.Amount ?? 0) * (inventory.Medicine.SellingPrice ?? 0);
                    await _repository.UpdatePrescriptionMedicineConsumptionAsync(pmc);
                }
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Lỗi khi chỉnh sửa trạng thái đơn thuốc: {ex.Message}");
        }
    }

    public async Task<List<MedicineInventoryDTO>> GetAllMedicinesInInventoryAsync()
    {
        var inventories = await _repository.GetAvailableMedicinesAsync();
        return inventories.Select(mi => new MedicineInventoryDTO
        {
            MedicineId = mi.MedicineId,
            MedicineName = mi.Medicine.MedicineName,
            ActiveIngredient = mi.Medicine.ActiveIngredient,
            Dosage = mi.Medicine.Dosage,
            DosageForm = mi.Medicine.DosageForm,
            MedicineInventoryId = mi.MedicineInventoryId,
            Quantity = mi.Quantity ?? 0,
            ExpiryDate = mi.ExpiryDate ?? DateTime.MinValue,
            IsBhyt = mi.Medicine.IsBhyt ?? false,
        }).ToList();
    }


    public async Task<List<PrescriptionDTO>> GetPrescriptionsByUserIdListAsync(int userId)
    {
        var prescriptions = await _repository.GetPrescriptionsByUserIdAsync(userId);
        return prescriptions.Select(p => new PrescriptionDTO
        {
            PrescriptionId = p.PrescriptionId,
            IssueDate = p.IssueDate ?? DateTime.MinValue,
            Status = p.Status ?? false,
            Note = p.Note ?? string.Empty,
            IsBhyt = p.IsBhyt ?? false,
            PatientName = p.MedicalRecordHistory?.MedicalRecord?.PatientName
        }).ToList();
    }
    public async Task<List<PrescriptionDTO>> GetAllPrescriptionsAsync()
    {
        var prescriptions = await _repository.GetAllPrescriptionsAsync();
        return prescriptions.Where(p =>p.Status == true)
            .Select(p => new PrescriptionDTO
        {
            PrescriptionId = p.PrescriptionId,
            IssueDate = p.IssueDate ?? DateTime.MinValue,
            Status = p.Status.Value,
            Note = p.Note ?? string.Empty,              
            IsBhyt = p.IsBhyt ?? false,
            PatientName = p.MedicalRecordHistory?.MedicalRecord?.PatientName
        }).ToList();
    }

    public async Task<PrescriptionDTO> GetPrescriptionByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
    {
        var prescription = await _repository.GetPrescriptionByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
        if (prescription == null)
            throw new Exception("Không tìm thấy đơn thuốc");

        return new PrescriptionDTO
        {
            PrescriptionId = prescription.PrescriptionId,
            IssueDate = prescription.IssueDate ?? DateTime.MinValue, 
            Status = prescription.Status ?? false,                  
            Note = prescription.Note ?? string.Empty,              
            IsBhyt = prescription.IsBhyt ?? false,
            PatientName = prescription.MedicalRecordHistory?.MedicalRecord?.PatientName
        };
    }

    public async Task<PrescriptionDetailDTO> GetPrescriptionDetailAsync(int prescriptionId)
    {
        var prescription = await _repository.GetPrescriptionDetailAsync(prescriptionId);
        if (prescription == null)
            throw new Exception("Không tìm thấy đơn thuốc");

        // Lấy danh sách PrescriptionMedicineConsumptions liên quan
        var prescriptionMedicineConsumptions = await _context.PrescriptionMedicineConsumptions
            .Where(pmc => pmc.PrescriptionId == prescriptionId)
            .Include(pmc => pmc.MedicineConsumtion)
            .ThenInclude(mc => mc.MedicineInventory)
            .ThenInclude(mi => mi.Medicine)
            .ToListAsync();

        // Tính tổng TotalPrice, chuyển double? sang decimal
        var totalPrice = prescriptionMedicineConsumptions.Sum(pmc =>
            pmc.TotalPrice.HasValue ? Convert.ToDecimal(pmc.TotalPrice.Value) : 0m);

        return new PrescriptionDetailDTO
        {
            PrescriptionId = prescription.PrescriptionId,
            IssueDate = prescription.IssueDate ?? DateTime.MinValue,
            Status = prescription.Status ?? false,
            Note = prescription.Note ?? string.Empty,
            UserName = prescription.User?.UserName ?? string.Empty,
            PatientName = prescription.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty,
            HealthInsurance = prescription.MedicalRecordHistory?.MedicalRecord?.HealthInsurance ?? string.Empty,
            DiagnoseConclusion = prescription.MedicalRecordHistory?.DiagnoseConclusion ?? string.Empty,
            MedicineConsumptions = prescriptionMedicineConsumptions.Select(pmc => new MedicineConsumptionDetailDTO
            {
                MedicineConsumptionId = pmc.MedicineConsumtion.MedicineConsumptionId, // Thêm ánh xạ này
                Amount = (int)(pmc.MedicineConsumtion.Amount ?? 0), // Chuyển từ double? sang int
                ConsumptionDate = pmc.MedicineConsumtion.ConsumptionDate ?? DateTime.MinValue,
                Note = pmc.MedicineConsumtion.Note ?? string.Empty,
                IsSpecialMedicine = pmc.MedicineConsumtion.IsSpecialMedicine ?? false,
                Status = pmc.MedicineConsumtion.Status ?? false,
                MedicineName = pmc.MedicineConsumtion.MedicineInventory?.Medicine?.MedicineName ?? string.Empty,
                TotalPrice = pmc.TotalPrice.HasValue ? Convert.ToDecimal(pmc.TotalPrice.Value) : 0m
            }).ToList(),
            TotalPrice = totalPrice
        };
    }

    // Tạo đơn thuốc kê ngoài(thuốc ko được bhyt chi trả)
    public async Task<int> CreatePrescriptionNoBHYTAsync(CreatePrescriptionNoBHYTDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Business Rule 3: Kiểm tra IssueDate
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Business Rule 4: Kiểm tra số lượng tối đa thuốc
            if (dto.MedicinesToAdd.Count > 10)
                throw new Exception("Một đơn thuốc không được chứa quá 10 loại thuốc!");

            // Kiểm tra trùng MedicineId
            var medicineIds = dto.MedicinesToAdd.Select(mc => mc.MedicineId).ToList();
            if (medicineIds.Distinct().Count() != medicineIds.Count)
                throw new Exception("Có thuốc bị trùng trong đơn thuốc. Vui lòng kiểm tra lại!");

            // Lấy danh sách thuốc hợp lệ (Status = true)
            var validMedicines = await _repository.GetMedicinesForSelectionNoBHYTAsync();

            // Tạo Prescription
            var prescription = new Prescription
            {
                MedicalRecordHistoryId = dto.MedicalRecordHistoryId,
                UserId = dto.UserId,
                IssueDate = dto.IssueDate,
                Status = false, // Mặc định là false (chưa xác nhận)
                Note = dto.Note,
                IsBhyt = dto.IsBhyt
            };
            var createdPrescription = await _repository.CreatePrescriptionNoBHYTAsync(prescription);

            // Tạo MedicinePrescription
            foreach (var medDto in dto.MedicinesToAdd)
            {
                // Kiểm tra xem MedicineId có trong danh sách thuốc hợp lệ không
                var medicine = validMedicines.FirstOrDefault(m => m.MedicineId == medDto.MedicineId);
                if (medicine == null)
                    throw new Exception($"Không tìm thấy thuốc với ID: {medDto.MedicineId} hoặc thuốc không hoạt động!");

                var medicinePrescription = new MedicinePrescription
                {
                    PrescriptionId = createdPrescription.PrescriptionId,
                    MedicineId = medDto.MedicineId,
                    Amount = medDto.Amount,
                    Note = medDto.Note
                };
                await _repository.CreateMedicinePrescriptionNoBHYTAsync(medicinePrescription);
            }

            await transaction.CommitAsync();
            return createdPrescription.PrescriptionId;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Lỗi khi tạo đơn thuốc: {ex.Message}");
        }
    }

    public async Task<List<Medicine>> GetMedicinesForSelectionNoBHYTAsync()
    {
        return await _repository.GetMedicinesForSelectionNoBHYTAsync();
    }

    // chỉnh sửa đơn thuốc kê ngoài((thuốc ko được bhyt chi trả))
    public async Task<int> EditPrescriptionNoBHYTAsync(int id, CreatePrescriptionNoBHYTDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Business Rule 3: Kiểm tra IssueDate
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Kiểm tra trạng thái đơn thuốc
            var prescription = await _repository.GetPrescriptionByIdAsync(id);
            if (prescription == null)
                throw new Exception($"Không tìm thấy đơn thuốc với ID: {id}");

            // Cập nhật thông tin đơn thuốc
            prescription.MedicalRecordHistoryId = dto.MedicalRecordHistoryId;
            prescription.UserId = dto.UserId;
            prescription.IssueDate = dto.IssueDate;
            prescription.Note = dto.Note;
            prescription.IsBhyt = dto.IsBhyt;
            await _repository.UpdatePrescriptionAsync(prescription);

            // Xóa các MedicinePrescription được chỉ định trong MedicineIdsToRemove
            foreach (var medicineId in dto.MedicineIdsToRemove)
            {
                await _repository.DeleteMedicinePrescriptionAsync(id, medicineId);
            }

            // Kiểm tra số lượng thuốc tối đa
            var existingMedicines = await _repository.GetMedicinePrescriptionsByPrescriptionIdAsync(id);
            if (existingMedicines.Count + dto.MedicinesToAdd.Count > 10)
                throw new Exception("Một đơn thuốc không được chứa quá 10 loại thuốc!");

            // Kiểm tra trùng MedicineId trong danh sách thêm mới
            var medicineIds = dto.MedicinesToAdd.Select(mc => mc.MedicineId).ToList();
            if (medicineIds.Distinct().Count() != medicineIds.Count)
                throw new Exception("Có thuốc bị trùng trong danh sách thêm mới. Vui lòng kiểm tra lại!");

            // Kiểm tra trùng với các MedicineId hiện có
            var existingMedicineIds = existingMedicines.Select(mp => mp.MedicineId).ToList();
            if (medicineIds.Any(id => existingMedicineIds.Contains(id)))
                throw new Exception("Có thuốc trong danh sách thêm mới đã tồn tại trong đơn thuốc. Vui lòng kiểm tra lại!");

            // Lấy danh sách thuốc hợp lệ (Status = true)
            var validMedicines = await _repository.GetMedicinesForSelectionNoBHYTAsync();

            // Thêm mới MedicinePrescription
            foreach (var medDto in dto.MedicinesToAdd)
            {
                // Kiểm tra xem MedicineId có trong danh sách thuốc hợp lệ không
                var medicine = validMedicines.FirstOrDefault(m => m.MedicineId == medDto.MedicineId);
                if (medicine == null)
                    throw new Exception($"Không tìm thấy thuốc với ID: {medDto.MedicineId} hoặc thuốc không hoạt động!");

                var medicinePrescription = new MedicinePrescription
                {
                    PrescriptionId = id,
                    MedicineId = medDto.MedicineId,
                    Amount = medDto.Amount,
                    Note = medDto.Note
                };
                await _repository.CreateMedicinePrescriptionNoBHYTAsync(medicinePrescription);
            }

            await transaction.CommitAsync();
            return id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Lỗi khi chỉnh sửa đơn thuốc: {ex.Message}");
        }
    }
    public async Task<List<PrescriptionDTO>> GetAllPrescriptionsNoBHYTAsync()
    {
        var prescriptions = await _repository.GetAllPrescriptionsNoBHYTAsync();
        return prescriptions.Where(p => p.Status == false)
            .Select(p => new PrescriptionDTO
            {
                PrescriptionId = p.PrescriptionId,
                IssueDate = p.IssueDate ?? DateTime.MinValue,
                Status = p.Status.Value,
                Note = p.Note ?? string.Empty,
                IsBhyt = p.IsBhyt ?? false,
                PatientName = p.MedicalRecordHistory?.MedicalRecord?.PatientName
            }).ToList();
    }

    public int GetTodayPrescriptionCount()
    {
        return _repository.CountTodayPrescriptions();
    }

}



    


