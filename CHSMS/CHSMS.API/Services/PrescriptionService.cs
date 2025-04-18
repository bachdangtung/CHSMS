using CHSMS.API.DTOs;
using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.MedicineInventory;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using Microsoft.EntityFrameworkCore;



public class PrescriptionService
{
    private readonly PrescriptionRepository _repository;

    private readonly SEP_TestContext _context;


    public PrescriptionService(PrescriptionRepository repository, SEP_TestContext context)

    {
        _repository = repository;
        _context = context;

    }

    public async Task<int> CreatePrescriptionAsync(int userId, int medicalRecordHistoryId, CreatePrescriptionDTO dto)
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

            // Tạo Prescription với Status mặc định là false
            var prescription = new Prescription
            {
                MedicalRecordHistoryId = medicalRecordHistoryId,
                UserId = userId,
                IssueDate = dto.IssueDate,
                Status = false, // Mặc định là false cho bác sĩ
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

            // Gán các giá trị từ DTO (MedicalRecordHistoryId và UserId đã được gán trong controller)
            prescription.MedicalRecordHistoryId = dto.MedicalRecordHistoryId;
            prescription.UserId = dto.UserId;
            prescription.IssueDate = dto.IssueDate;
            prescription.Note = dto.Note;
            prescription.IsBhyt = dto.IsBhyt;
            await _repository.UpdatePrescriptionAsync(prescription);

            // Xóa các MedicineConsumption được chỉ định
            foreach (var consumptionId in dto.MedicineConsumptionIdsToRemove)
            {
                var pmc = await _repository.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(consumptionId);
                if (pmc != null)
                {
                    await _repository.DeletePrescriptionMedicineConsumptionAsync(pmc.PrescriptionId, pmc.MedicineConsumtionId);
                    await _repository.DeleteMedicineConsumptionAsync(consumptionId);
                }
            }

            // Kiểm tra số lượng thuốc trong đơn
            var existingConsumptions = await _repository.GetMedicineConsumptionsByPrescriptionIdAsync(dto.PrescriptionId);
            if (existingConsumptions.Count() + dto.MedicineConsumptionsToAdd.Count > 10)
                throw new Exception("Một đơn thuốc không được chứa quá 10 loại thuốc!");

            // Kiểm tra thuốc bị trùng
            var medicineInventoryIds = dto.MedicineConsumptionsToAdd.Select(mc => mc.MedicineInventoryId).ToList();
            if (medicineInventoryIds.Distinct().Count() != medicineInventoryIds.Count)
                throw new Exception("Có thuốc bị trùng trong danh sách thêm mới. Vui lòng kiểm tra lại!");

            // Thêm các MedicineConsumption mới
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

            // Business Rule: Chỉ cho phép chỉnh sửa trong cùng ngày với IssueDate
            if (!prescription.IssueDate.HasValue || prescription.IssueDate.Value.Date != DateTime.Now.Date)
                throw new Exception("Chỉ được chỉnh sửa trạng thái đơn thuốc trong ngày phát hành đơn thuốc!");

            bool hasAnyConsumptionDispensed = false; // Biến để kiểm tra xem có MedicineConsumption nào được cấp phát không

            foreach (var statusDto in dto.MedicineConsumptionStatuses)
            {
                var consumption = await _repository.GetMedicineConsumptionByIdAsync(statusDto.MedicineConsumptionId);
                if (consumption == null)
                    throw new Exception($"Không tìm thấy MedicineConsumption với ID: {statusDto.MedicineConsumptionId}");

                if (!statusDto.Status) // Kiểm tra rollback
                {
                    // Kiểm tra chỉ được rollback 1 lần
                    if (!consumption.Status.HasValue || !consumption.Status.Value)
                        throw new Exception($"MedicineConsumption ID {statusDto.MedicineConsumptionId} đã được rollback trước đó hoặc chưa được phát thuốc, không thể rollback!");
                }

                // Cập nhật Status
                consumption.Status = statusDto.Status;
                await _repository.UpdateMedicineConsumptionAsync(consumption);

                if (statusDto.Status) // Phát thuốc
                {
                    hasAnyConsumptionDispensed = true; // Đánh dấu có ít nhất một MedicineConsumption được cấp phát

                    if (!consumption.MedicineInventoryId.HasValue)
                        throw new Exception($"MedicineInventoryId không được để trống trong MedicineConsumption với ID: {consumption.MedicineConsumptionId}");

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
                        throw new Exception($"Số lượng tồn kho của thuốc ID {consumption.MedicineInventoryId} dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi phát thuốc!");

                    await _repository.UpdateMedicineInventoryAsync(inventory);

                    // Tính TotalPrice
                    var pmc = await _repository.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(consumption.MedicineConsumptionId);
                    pmc.TotalPrice = (consumption.Amount ?? 0) * (inventory.Medicine.SellingPrice ?? 0);
                    await _repository.UpdatePrescriptionMedicineConsumptionAsync(pmc);
                }
                else // Rollback (Status = false)
                {
                    if (!consumption.MedicineInventoryId.HasValue)
                        throw new Exception($"MedicineInventoryId không được để trống trong MedicineConsumption với ID: {consumption.MedicineConsumptionId}");

                    var inventory = await _repository.GetMedicineInventoryByIdAsync(consumption.MedicineInventoryId.Value);
                    if (inventory == null)
                        throw new Exception($"Không tìm thấy kho thuốc với ID: {consumption.MedicineInventoryId}");

                    // Hoàn lại Quantity
                    inventory.Quantity += (consumption.Amount ?? 0);
                    await _repository.UpdateMedicineInventoryAsync(inventory);

                    // Đặt lại TotalPrice
                    var pmc = await _repository.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(consumption.MedicineConsumptionId);
                    pmc.TotalPrice = 0; // Hoặc giá trị ban đầu nếu có
                    await _repository.UpdatePrescriptionMedicineConsumptionAsync(pmc);
                }
            }

            // Cập nhật trạng thái Prescription: true nếu có ít nhất một MedicineConsumption được cấp phát, false nếu không
            prescription.Status = hasAnyConsumptionDispensed;
            await _repository.UpdatePrescriptionAsync(prescription);

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

    // lấy danh sách đơn thuốc có bhyt
    public async Task<List<PrescriptionDTO>> GetAllPrescriptionsAsync()
    {
        var prescriptions = await _repository.GetAllPrescriptionsAsync();
        return prescriptions.Where(p => p.IsBhyt == true)
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

    // lấy danh sách đơn thuốc ko có bhyt
    public async Task<List<PrescriptionDTO>> GetAllPrescriptionsNoBHYTAsync()
    {
        var prescriptions = await _repository.GetAllPrescriptionsNoBHYTAsync();
        return prescriptions.Where(p => p.IsBhyt == false)
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

    // Lấy đơn thuốc theo Lịch sử bệnh án
    public async Task<List<PrescriptionDTO>> GetPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
    {
        var prescriptions = await _repository.GetPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
        if (prescriptions == null || !prescriptions.Any())
            throw new Exception("Không tìm thấy đơn thuốc");

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
            FullName = prescription.User?.Fullname ?? string.Empty,
            PatientName = prescription.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty,
            Gender = prescription.MedicalRecordHistory?.MedicalRecord?.Gender ?? string.Empty,
            Dob = prescription.MedicalRecordHistory?.MedicalRecord?.Dob?? DateTime.MinValue,
            Address = prescription.MedicalRecordHistory?.MedicalRecord.Address ?? string.Empty,
            HealthInsurance = prescription.MedicalRecordHistory?.MedicalRecord?.HealthInsurance ?? string.Empty,
            DiagnoseConclusion = prescription.MedicalRecordHistory?.DiagnoseConclusion ?? string.Empty,
            IsBhyt = prescription.IsBhyt ?? false,
            MedicineConsumptions = prescriptionMedicineConsumptions.Select(pmc => new MedicineConsumptionDetailDTO
            {
                MedicineConsumptionId = pmc.MedicineConsumtion.MedicineConsumptionId, // Thêm ánh xạ này
                Amount = (int)(pmc.MedicineConsumtion.Amount ?? 0),
                ConsumptionDate = pmc.MedicineConsumtion.ConsumptionDate ?? DateTime.MinValue,
                Note = pmc.MedicineConsumtion.Note ?? string.Empty,
                IsSpecialMedicine = pmc.MedicineConsumtion.IsSpecialMedicine ?? false,
                Status = pmc.MedicineConsumtion.Status ?? false,
                MedicineName = pmc.MedicineConsumtion.MedicineInventory?.Medicine?.MedicineName ?? string.Empty,
                DosageForm = pmc.MedicineConsumtion.MedicineInventory?.Medicine?.DosageForm?? string.Empty,
                BatchNumber = pmc.MedicineConsumtion.MedicineInventory?.BatchNumber ?? string.Empty,
                TransactionDate = pmc.MedicineConsumtion.MedicineInventory?.TransactionDate??DateTime.MinValue,
                ExpiryDate = pmc.MedicineConsumtion.MedicineInventory?.ExpiryDate ?? DateTime.MinValue,
                Quantity = pmc.MedicineConsumtion.MedicineInventory?.Quantity ?? 0,
                IsBhyt = pmc.MedicineConsumtion.MedicineInventory.Medicine?.IsBhyt ?? false,
                TotalPrice = pmc.TotalPrice.HasValue ? Convert.ToDecimal(pmc.TotalPrice.Value) : 0m
            }).ToList(),
            TotalPrice = totalPrice
        };
    }

    public int GetTodayPrescriptionCount()
    {
        return _repository.CountTodayPrescriptions();
    }

}






