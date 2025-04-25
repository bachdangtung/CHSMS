using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UseMedicalSupplyRepository : IUseMedicalSupplyRepository
{
    private readonly SEP_TestContext _context;

    public UseMedicalSupplyRepository(SEP_TestContext context)
    {
        _context = context;
    }

    public async Task<UseMedicalSupply> CreateUseMedicalSupplyAsync(UseMedicalSupply useMedicalSupply)
    {
        _context.UseMedicalSupplies.Add(useMedicalSupply);
        await _context.SaveChangesAsync();
        return useMedicalSupply;
    }

    public async Task<MedicalSupplyInventory> GetMedicalSupplyInventoryByIdAsync(int SupplyInventoryId)
    {
        return await _context.MedicalSupplyInventories
            .Include(msi => msi.MedicalSupply) // Load thông tin MedicalSupply liên quan
            .FirstOrDefaultAsync(msi => msi.SupplyInventoryId == SupplyInventoryId);
    }

    public async Task<MedicalSupplyConsumption> CreateMedicalSupplyConsumptionAsync(MedicalSupplyConsumption consumption)
    {
        _context.MedicalSupplyConsumptions.Add(consumption);
        await _context.SaveChangesAsync();
        return consumption;
    }

    public async Task<UseMedicalSuppliesMedicalSupplyConsumption> CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(UseMedicalSuppliesMedicalSupplyConsumption umsmsc)
    {
        _context.UseMedicalSuppliesMedicalSupplyConsumptions.Add(umsmsc);
        await _context.SaveChangesAsync();
        return umsmsc;
    }

    public async Task<List<MedicalSupplyInventory>> GetAvailableMedicalSuppliesAsync()
    {
        var currentDate = DateTime.UtcNow; // Ngày hiện tại
        return await _context.MedicalSupplyInventories
            .Include(msi => msi.MedicalSupply)
            .Where(msi => msi.Quantity > 0 && msi.ExpiryDate > currentDate && msi.MedicalSupply.Status == true)
            .ToListAsync();
    }

    public async Task UpdateMedicalSupplyInventoryAsync(MedicalSupplyInventory inventory)
    {
        _context.MedicalSupplyInventories.Update(inventory);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UseMedicalSupply>> GetUseMedicalSuppliesByUserIdAsync(int userId)
    {
        return await _context.UseMedicalSupplies
            .Include(ums => ums.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .Where(ums => ums.UserId == userId)
            .OrderByDescending(ums => ums.IssueDate)
            .ToListAsync();
    }

    public async Task<List<UseMedicalSupply>> GetAllUseMedicalSuppliesAsync()
    {
        return await _context.UseMedicalSupplies
            .Include(ums => ums.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .OrderByDescending(ums => ums.IssueDate)
            .ToListAsync();
    }

    public async Task<List<UseMedicalSupply>> GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
    {
        return await _context.UseMedicalSupplies
            .Include(ums => ums.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .Where(ums => ums.MedicalRecordHistoryId == medicalRecordHistoryId)
            .ToListAsync();
    }

    public async Task<UseMedicalSupply> GetUseMedicalSupplyDetailAsync(int useMedicalSupplyId)
    {
        var useMedicalSupplyDetail = await _context.UseMedicalSuppliesMedicalSupplyConsumptions
            .Where(umsmsc => umsmsc.UseMedicalSupplieId == useMedicalSupplyId)
            .Include(umsmsc => umsmsc.UseMedicalSupplie)
            .ThenInclude(ums => ums.User)
            .Include(ums => ums.UseMedicalSupplie)
            .ThenInclude(ums => ums.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .Include(umsmsc => umsmsc.Msconsumption)
            .ThenInclude(msc => msc.MedicalSupplyInventory)
            .ThenInclude(msi => msi.MedicalSupply)
            .Select(umsmsc => umsmsc.UseMedicalSupplie)
            .FirstOrDefaultAsync();
        return useMedicalSupplyDetail;
    }


    public async Task<UseMedicalSupply> GetUseMedicalSupplyByIdAsync(int useMedicalSupplyId)
    {
        return await _context.UseMedicalSupplies
            .Include(ums => ums.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .FirstOrDefaultAsync(ums => ums.UseMedicalSupplieId == useMedicalSupplyId);
    }

    public async Task<List<MedicalSupplyConsumption>> GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(int useMedicalSupplyId)
    {
        return await _context.UseMedicalSuppliesMedicalSupplyConsumptions
            .Where(umsmsc => umsmsc.UseMedicalSupplieId == useMedicalSupplyId)
            .Include(umsmsc => umsmsc.Msconsumption)
            .Select(umsmsc => umsmsc.Msconsumption)
            .ToListAsync();
    }

    public async Task UpdateUseMedicalSupplyAsync(UseMedicalSupply useMedicalSupply)
    {
        _context.UseMedicalSupplies.Update(useMedicalSupply);
        await _context.SaveChangesAsync();
    }

    public async Task<UseMedicalSuppliesMedicalSupplyConsumption> GetUseMedicalSuppliesMedicalSupplyConsumptionByConsumptionIdAsync(int msConsumptionId)
    {
        return await _context.UseMedicalSuppliesMedicalSupplyConsumptions
            .FirstOrDefaultAsync(umsmsc => umsmsc.MsconsumptionId == msConsumptionId);
    }

    public async Task DeleteUseMedicalSuppliesMedicalSupplyConsumptionAsync(int useMedicalSupplyId, int msConsumptionId)
    {
        var umsmsc = await _context.UseMedicalSuppliesMedicalSupplyConsumptions
            .FirstOrDefaultAsync(ums => ums.UseMedicalSupplieId == useMedicalSupplyId && ums.MsconsumptionId == msConsumptionId);
        if (umsmsc != null)
        {
            _context.UseMedicalSuppliesMedicalSupplyConsumptions.Remove(umsmsc);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteMedicalSupplyConsumptionAsync(int consumptionId)
    {
        var consumption = await _context.MedicalSupplyConsumptions.FindAsync(consumptionId);
        if (consumption != null)
        {
            _context.MedicalSupplyConsumptions.Remove(consumption);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<MedicalSupplyConsumption> GetMedicalSupplyConsumptionByIdAsync(int msConsumptionId)
    {
        return await _context.MedicalSupplyConsumptions
            .FirstOrDefaultAsync(msc => msc.MsconsumptionId == msConsumptionId);
    }

    public async Task UpdateMedicalSupplyConsumptionAsync(MedicalSupplyConsumption consumption)
    {
        _context.MedicalSupplyConsumptions.Update(consumption);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUseMedicalSuppliesMedicalSupplyConsumptionAsync(UseMedicalSuppliesMedicalSupplyConsumption umsmsc)
    {
        _context.UseMedicalSuppliesMedicalSupplyConsumptions.Update(umsmsc);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UseMedicalSuppliesMedicalSupplyConsumption>> GetAllMedicalSupplyConsumptionsAsync()
    {
        return await _context.UseMedicalSuppliesMedicalSupplyConsumptions
            .Include(umsmsc => umsmsc.Msconsumption)
                .ThenInclude(msc => msc.MedicalSupplyInventory)
                    .ThenInclude(msi => msi.MedicalSupply)
            .Include(umsmsc => umsmsc.UseMedicalSupplie)
            .Where(umsmsc => umsmsc.Msconsumption.Status == true)
            .OrderByDescending(umsmsc => umsmsc.Msconsumption.ConsumptionDate)
            .ToListAsync();
    }
}