using CHSMS.API.DTOs;
using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.DTOs.MedicalSupplyConsumption;
using CHSMS.API.DTOs.UseMedicalSupply;
using CHSMS.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UseMedicalSupplyService
{
    private readonly UseMedicalSupplyRepository _repository;
    private readonly SEP_TestContext _context;

    public UseMedicalSupplyService(UseMedicalSupplyRepository repository, SEP_TestContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<int> CreateUseMedicalSupplyAsync(int userId, int medicalRecordHistoryId, CreateUseMedicalSupplyDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Business Rule 3: Kiểm tra IssueDate
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Business Rule 4: Kiểm tra số lượng tối đa loại vật tư (dựa trên MedicalSupplyId)
            var medicalSupplyIds = new List<int>();
            foreach (var msDto in dto.MedicalSupplyConsumptions)
            {
                var inventory = await _repository.GetMedicalSupplyInventoryByIdAsync(msDto.MedicalSupplyInventoryId);
                if (inventory == null)
                    throw new Exception($"Không tìm thấy kho vật tư với ID: {msDto.MedicalSupplyInventoryId}");
                medicalSupplyIds.Add(inventory.MedicalSupplyId);
            }
            if (medicalSupplyIds.Distinct().Count() > 10)
                throw new Exception("Một đơn vật tư không được chứa quá 10 loại vật tư!");

            // Kiểm tra trùng MedicalSupplyInventoryId
            var medicalSupplyInventoryIds = dto.MedicalSupplyConsumptions.Select(mc => mc.MedicalSupplyInventoryId).ToList();
            if (medicalSupplyInventoryIds.Distinct().Count() != medicalSupplyInventoryIds.Count)
                throw new Exception("Có vật tư bị trùng trong đơn vật tư. Vui lòng kiểm tra lại!");

            // Tạo UseMedicalSupply với Status mặc định là false
            var useMedicalSupply = new UseMedicalSupply
            {
                MedicalRecordHistoryId = medicalRecordHistoryId,
                UserId = userId,
                IssueDate = dto.IssueDate,
                Status = false, // Mặc định là false
                Note = dto.Note
            };
            var createdUseMedicalSupply = await _repository.CreateUseMedicalSupplyAsync(useMedicalSupply);

            // Tạo MedicalSupplyConsumption
            foreach (var msDto in dto.MedicalSupplyConsumptions)
            {
                var inventory = await _repository.GetMedicalSupplyInventoryByIdAsync(msDto.MedicalSupplyInventoryId);
                if (inventory == null)
                    throw new Exception($"Không tìm thấy kho vật tư với ID: {msDto.MedicalSupplyInventoryId}");

                // Kiểm tra số lượng và hạn sử dụng
                if (msDto.Amount > (inventory.Quantity ?? 0))
                    throw new Exception($"Số lượng yêu cầu vượt quá tồn kho");
                if (msDto.ConsumptionDate > (inventory.ExpiryDate ?? DateTime.MaxValue) || dto.IssueDate > (inventory.ExpiryDate ?? DateTime.MaxValue))
                    throw new Exception($"Ngày sử dụng vượt quá hạn sử dụng");

                // Business Rule 8: Kiểm tra số lượng tồn kho tối thiểu (cho bác sĩ)
                const int minimumQuantity = 10;
                if ((inventory.Quantity ?? 0) - msDto.Amount < minimumQuantity)
                {
                    throw new Exception($"Số lượng tồn kho của vật tư ID {msDto.MedicalSupplyInventoryId} sẽ dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi tạo đơn vật tư!");
                }

                var consumption = new MedicalSupplyConsumption
                {
                    MedicalSupplyInventoryId = msDto.MedicalSupplyInventoryId,
                    Amount = msDto.Amount,
                    ConsumptionDate = msDto.ConsumptionDate,
                    Note = msDto.Note,
                    Status = false // Mặc định là false
                };
                var createdConsumption = await _repository.CreateMedicalSupplyConsumptionAsync(consumption);

                var umsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
                {
                    UseMedicalSupplieId = createdUseMedicalSupply.UseMedicalSupplieId,
                    MsconsumptionId = createdConsumption.MsconsumptionId,
                    TotalPrice = 0 // Chưa tính TotalPrice
                };
                await _repository.CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(umsmsc);
            }

            await transaction.CommitAsync();
            return createdUseMedicalSupply.UseMedicalSupplieId;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Chi tiết lỗi Entity Framework: " + ex.ToString());
            if (ex.InnerException != null)
            {
                Console.WriteLine("INNER: " + ex.InnerException.Message);
            }
            throw new Exception($"Lỗi khi tạo đơn vật tư: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task EditUseMedicalSupplyForDoctorAsync(EditUseMedicalSupplyForDoctorDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Kiểm tra ngày phát hành không được trong tương lai
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Kiểm tra trạng thái đơn vật tư
            var useMedicalSupply = await _repository.GetUseMedicalSupplyByIdAsync(dto.UseMedicalSupplyId);
            if (useMedicalSupply == null)
                throw new Exception($"Không tìm thấy đơn vật tư với ID: {dto.UseMedicalSupplyId}");

            var consumptions = await _repository.GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(dto.UseMedicalSupplyId);
            if (consumptions.All(c => c.Status ?? false))
                throw new Exception("Đơn vật tư đã được xác nhận hoàn tất, không thể chỉnh sửa!");

            // Gán các giá trị từ DTO
            useMedicalSupply.MedicalRecordHistoryId = dto.MedicalRecordHistoryId;
            useMedicalSupply.UserId = dto.UserId;
            useMedicalSupply.IssueDate = dto.IssueDate;
            useMedicalSupply.Note = dto.Note;
            await _repository.UpdateUseMedicalSupplyAsync(useMedicalSupply);

            // Xóa các MedicalSupplyConsumption được chỉ định
            foreach (var consumptionId in dto.MedicalSupplyConsumptionIdsToRemove)
            {
                var umsmsc = await _repository.GetUseMedicalSuppliesMedicalSupplyConsumptionByConsumptionIdAsync(consumptionId);
                if (umsmsc != null)
                {
                    await _repository.DeleteUseMedicalSuppliesMedicalSupplyConsumptionAsync(umsmsc.UseMedicalSupplieId, umsmsc.MsconsumptionId);
                    await _repository.DeleteMedicalSupplyConsumptionAsync(consumptionId);
                }
            }

            // Kiểm tra số lượng vật tư trong đơn
            var existingConsumptions = await _repository.GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(dto.UseMedicalSupplyId);
            if (existingConsumptions.Count() + dto.MedicalSupplyConsumptionsToAdd.Count > 10)
                throw new Exception("Một đơn vật tư không được chứa quá 10 loại vật tư!");

            // Kiểm tra vật tư bị trùng
            var medicalSupplyInventoryIds = dto.MedicalSupplyConsumptionsToAdd.Select(mc => mc.MedicalSupplyInventoryId).ToList();
            if (medicalSupplyInventoryIds.Distinct().Count() != medicalSupplyInventoryIds.Count)
                throw new Exception("Có vật tư bị trùng trong danh sách thêm mới. Vui lòng kiểm tra lại!");

            // Thêm các MedicalSupplyConsumption mới
            foreach (var msDto in dto.MedicalSupplyConsumptionsToAdd)
            {
                var inventory = await _repository.GetMedicalSupplyInventoryByIdAsync(msDto.MedicalSupplyInventoryId);
                if (inventory == null)
                    throw new Exception($"Không tìm thấy kho vật tư với ID: {msDto.MedicalSupplyInventoryId}");

                if (msDto.Amount > (inventory.Quantity ?? 0))
                    throw new Exception($"Số lượng yêu cầu vượt quá tồn kho");
                if (msDto.ConsumptionDate > (inventory.ExpiryDate ?? DateTime.MaxValue) || dto.IssueDate > (inventory.ExpiryDate ?? DateTime.MaxValue))
                    throw new Exception($"Ngày sử dụng vượt quá hạn sử dụng");

                // Business Rule 8: Kiểm tra số lượng tồn kho tối thiểu (cho bác sĩ)
                const int minimumQuantity = 10;
                if ((inventory.Quantity ?? 0) - msDto.Amount < minimumQuantity)
                {
                    throw new Exception($"Số lượng tồn kho của vật tư ID {msDto.MedicalSupplyInventoryId} sẽ dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi thêm vào đơn vật tư!");
                }

                var consumption = new MedicalSupplyConsumption
                {
                    MedicalSupplyInventoryId = msDto.MedicalSupplyInventoryId,
                    Amount = msDto.Amount,
                    ConsumptionDate = msDto.ConsumptionDate,
                    Note = msDto.Note,
                    Status = false // Mặc định là false
                };
                var createdConsumption = await _repository.CreateMedicalSupplyConsumptionAsync(consumption);

                var umsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
                {
                    UseMedicalSupplieId = dto.UseMedicalSupplyId,
                    MsconsumptionId = createdConsumption.MsconsumptionId,
                    TotalPrice = 0 // Chưa tính TotalPrice
                };
                await _repository.CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(umsmsc);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Lỗi khi chỉnh sửa đơn vật tư: {ex.Message}");
        }
    }

    public async Task EditUseMedicalSupplyForPharmacistAsync(EditUseMedicalSupplyForPharmacistDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Lấy UseMedicalSupply
            var useMedicalSupply = await _repository.GetUseMedicalSupplyByIdAsync(dto.UseMedicalSupplyId);
            if (useMedicalSupply == null)
                throw new Exception($"Không tìm thấy đơn vật tư với ID: {dto.UseMedicalSupplyId}");

            // Business Rule: Chỉ cho phép chỉnh sửa trong cùng ngày với IssueDate
            if (!useMedicalSupply.IssueDate.HasValue || useMedicalSupply.IssueDate.Value.Date != DateTime.UtcNow.Date)
                throw new Exception("Chỉ được chỉnh sửa trạng thái đơn vật tư trong ngày phát hành đơn vật tư!");

            bool hasAnyConsumptionDispensed = false; // Biến để kiểm tra xem có MedicalSupplyConsumption nào được cấp phát không

            foreach (var statusDto in dto.MedicalSupplyConsumptionStatuses)
            {
                var consumption = await _repository.GetMedicalSupplyConsumptionByIdAsync(statusDto.MedicalSupplyConsumptionId);
                if (consumption == null)
                    throw new Exception($"Không tìm thấy MedicalSupplyConsumption với ID: {statusDto.MedicalSupplyConsumptionId}");

                if (!statusDto.Status) // Kiểm tra rollback
                {
                    // Kiểm tra chỉ được rollback 1 lần
                    if (!consumption.Status.HasValue || !consumption.Status.Value)
                        throw new Exception($"MedicalSupplyConsumption ID {statusDto.MedicalSupplyConsumptionId} đã được rollback trước đó hoặc chưa được phát vật tư, không thể rollback!");
                }

                // Cập nhật Status
                consumption.Status = statusDto.Status;
                await _repository.UpdateMedicalSupplyConsumptionAsync(consumption);

                if (statusDto.Status) // Phát vật tư
                {
                    hasAnyConsumptionDispensed = true; // Đánh dấu có ít nhất một MedicalSupplyConsumption được cấp phát

                    if (consumption.MedicalSupplyInventoryId <= 0)
                        throw new Exception($"MedicalSupplyInventoryId không được để trống trong MedicalSupplyConsumption với ID: {statusDto.MedicalSupplyConsumptionId}");

                    var inventory = await _repository.GetMedicalSupplyInventoryByIdAsync(consumption.MedicalSupplyInventoryId);
                    if (inventory == null)
                        throw new Exception($"Không tìm thấy kho vật tư với ID: {consumption.MedicalSupplyInventoryId}");

                    // Trừ Quantity
                    inventory.Quantity -= (consumption.Amount ?? 0);
                    if (inventory.Quantity < 0)
                        throw new Exception($"Số lượng tồn kho không đủ để phát vật tư!");

                    // Kiểm tra số lượng tồn kho tối thiểu (cho dược sĩ)
                    const int minimumQuantity = 10;
                    if (inventory.Quantity < minimumQuantity)
                        throw new Exception($"Số lượng tồn kho của vật tư ID {consumption.MedicalSupplyInventoryId} dưới ngưỡng tối thiểu ({minimumQuantity}) sau khi phát vật tư!");

                    await _repository.UpdateMedicalSupplyInventoryAsync(inventory);

                    // Tính TotalPrice
                    var umsmsc = await _repository.GetUseMedicalSuppliesMedicalSupplyConsumptionByConsumptionIdAsync(consumption.MsconsumptionId);
                    umsmsc.TotalPrice = (consumption.Amount ?? 0) * (inventory.MedicalSupply?.SellingPrice ?? 0);
                    await _repository.UpdateUseMedicalSuppliesMedicalSupplyConsumptionAsync(umsmsc);
                }
                else // Rollback (Status = false)
                {
                    if (consumption.MedicalSupplyInventoryId <= 0)
                        throw new Exception($"MedicalSupplyInventoryId không được để trống trong MedicalSupplyConsumption với ID: {statusDto.MedicalSupplyConsumptionId}");

                    var inventory = await _repository.GetMedicalSupplyInventoryByIdAsync(consumption.MedicalSupplyInventoryId);
                    if (inventory == null)
                        throw new Exception($"Không tìm thấy kho vật tư với ID: {consumption.MedicalSupplyInventoryId}");

                    // Hoàn lại Quantity
                    inventory.Quantity += (consumption.Amount ?? 0);
                    await _repository.UpdateMedicalSupplyInventoryAsync(inventory);

                    // Đặt lại TotalPrice
                    var umsmsc = await _repository.GetUseMedicalSuppliesMedicalSupplyConsumptionByConsumptionIdAsync(consumption.MsconsumptionId);
                    umsmsc.TotalPrice = 0; // Hoặc giá trị ban đầu nếu có
                    await _repository.UpdateUseMedicalSuppliesMedicalSupplyConsumptionAsync(umsmsc);
                }
            }

            // Cập nhật trạng thái UseMedicalSupply: true nếu có ít nhất một MedicalSupplyConsumption được cấp phát, false nếu không
            useMedicalSupply.Status = hasAnyConsumptionDispensed;
            await _repository.UpdateUseMedicalSupplyAsync(useMedicalSupply);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Ghi log lỗi
            Console.WriteLine("Chi tiết lỗi: " + ex.ToString());
            if (ex.InnerException != null)
            {
                Console.WriteLine("INNER: " + ex.InnerException.Message);
            }
            throw new InvalidOperationException($"Lỗi khi chỉnh sửa trạng thái đơn vật tư: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    public async Task<List<MedicalSupplyInventoryforUseDTO>> GetAllMedicalSuppliesInInventoryAsync()
    {
        var inventories = await _repository.GetAvailableMedicalSuppliesAsync();
        return inventories.Select(msi => new MedicalSupplyInventoryforUseDTO
        {
            MedicalSupplyId = msi.MedicalSupplyId,
            MedicalSupplyName = msi.MedicalSupply?.MedicalSupplyName ?? string.Empty,
            MedicalSupplyInventoryId = msi.SupplyInventoryId,
            Quantity = msi.Quantity ?? 0,
            ExpiryDate = msi.ExpiryDate ?? DateTime.MinValue
        }).ToList();
    }

    public async Task<List<UseMedicalSupplyDTO>> GetUseMedicalSuppliesByUserIdListAsync(int userId)
    {
        var useMedicalSupplies = await _repository.GetUseMedicalSuppliesByUserIdAsync(userId);
        return useMedicalSupplies.Select(ums => new UseMedicalSupplyDTO
        {
            UseMedicalSupplyId = ums.UseMedicalSupplieId,
            IssueDate = ums.IssueDate ?? DateTime.MinValue,
            Status = ums.Status ?? false,
            Note = ums.Note ?? string.Empty,
            PatientName = ums.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty
        }).ToList();
    }

    public async Task<List<UseMedicalSupplyDTO>> GetAllUseMedicalSuppliesAsync()
    {
        var useMedicalSupplies = await _repository.GetAllUseMedicalSuppliesAsync();
        return useMedicalSupplies.Select(ums => new UseMedicalSupplyDTO
        {
            UseMedicalSupplyId = ums.UseMedicalSupplieId,
            IssueDate = ums.IssueDate ?? DateTime.MinValue,
            Status = ums.Status ?? false,
            Note = ums.Note ?? string.Empty,
            PatientName = ums.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty
        }).ToList();
    }

    public async Task<List<UseMedicalSupplyDTO>> GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
    {
        var useMedicalSupplies = await _repository.GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
        if (useMedicalSupplies == null || !useMedicalSupplies.Any())
            throw new Exception("Không tìm thấy đơn vật tư");

        return useMedicalSupplies.Select(ums => new UseMedicalSupplyDTO
        {
            UseMedicalSupplyId = ums.UseMedicalSupplieId,
            IssueDate = ums.IssueDate ?? DateTime.MinValue,
            Status = ums.Status ?? false,
            Note = ums.Note ?? string.Empty,
            PatientName = ums.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty
        }).ToList();
    }

    public async Task<UseMedicalSupplyDetailDTO> GetUseMedicalSupplyDetailAsync(int useMedicalSupplyId)
    {
        var useMedicalSupply = await _repository.GetUseMedicalSupplyDetailAsync(useMedicalSupplyId);
        if (useMedicalSupply == null)
            throw new Exception("Không tìm thấy đơn vật tư");

        // Lấy danh sách UseMedicalSuppliesMedicalSupplyConsumptions liên quan
        var useMedicalSuppliesConsumptions = await _context.UseMedicalSuppliesMedicalSupplyConsumptions
            .Where(umsmsc => umsmsc.UseMedicalSupplieId == useMedicalSupplyId)
            .Include(umsmsc => umsmsc.Msconsumption)
            .ThenInclude(msc => msc.MedicalSupplyInventory)
            .ThenInclude(msi => msi.MedicalSupply)
            .ToListAsync();

        // Tính tổng TotalPrice, chuyển double? sang decimal
        var totalPrice = useMedicalSuppliesConsumptions.Sum(umsmsc =>
            umsmsc.TotalPrice.HasValue ? Convert.ToDecimal(umsmsc.TotalPrice.Value) : 0m);

        return new UseMedicalSupplyDetailDTO
        {
            UseMedicalSupplyId = useMedicalSupply.UseMedicalSupplieId,
            IssueDate = useMedicalSupply.IssueDate ?? DateTime.MinValue,
            Status = useMedicalSupply.Status ?? false,
            Note = useMedicalSupply.Note ?? string.Empty,
            FullName = useMedicalSupply.User?.Fullname ?? string.Empty,
            PatientName = useMedicalSupply.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty,
            Gender = useMedicalSupply.MedicalRecordHistory?.MedicalRecord?.Gender ?? string.Empty,
            Dob = useMedicalSupply.MedicalRecordHistory?.MedicalRecord?.Dob ?? DateTime.MinValue,
            Address = useMedicalSupply.MedicalRecordHistory?.MedicalRecord?.Address ?? string.Empty,
            HealthInsurance = useMedicalSupply.MedicalRecordHistory?.MedicalRecord?.HealthInsurance ?? string.Empty,
            DiagnoseConclusion = useMedicalSupply.MedicalRecordHistory?.DiagnoseConclusion ?? string.Empty,
            MedicalSupplyConsumptions = useMedicalSuppliesConsumptions.Select(umsmsc => new MedicalSupplyConsumptionDetailDTO
            {
                MedicalSupplyConsumptionId = umsmsc.Msconsumption.MsconsumptionId,
                MedicalSupplyId = umsmsc.Msconsumption.MedicalSupplyInventory.MedicalSupplyId,
                Amount = (int)(umsmsc.Msconsumption.Amount ?? 0),
                ConsumptionDate = umsmsc.Msconsumption.ConsumptionDate ?? DateTime.MinValue,
                Note = umsmsc.Msconsumption.Note ?? string.Empty,
                Status = umsmsc.Msconsumption.Status ?? false,
                MedicalSupplyName = umsmsc.Msconsumption.MedicalSupplyInventory?.MedicalSupply?.MedicalSupplyName ?? string.Empty,
                BatchNumber = umsmsc.Msconsumption.MedicalSupplyInventory?.BatchNumber ?? string.Empty,
                TransactionDate = umsmsc.Msconsumption.MedicalSupplyInventory?.TransactionDate ?? DateTime.MinValue,
                ExpiryDate = umsmsc.Msconsumption.MedicalSupplyInventory?.ExpiryDate ?? DateTime.MinValue,
                Quantity = umsmsc.Msconsumption.MedicalSupplyInventory?.Quantity ?? 0,
                TotalPrice = umsmsc.TotalPrice.HasValue ? Convert.ToDecimal(umsmsc.TotalPrice.Value) : 0m
            }).ToList(),
            TotalPrice = totalPrice
        };
    }
}