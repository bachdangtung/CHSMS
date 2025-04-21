using CHSMS.API.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using CHSMS.API.DTOs;
using CHSMS.API.DTOs.Prescription;

public class PrescriptionRepository
{
    private readonly SEP_TestContext _context;

    public PrescriptionRepository(SEP_TestContext context)
    {
        _context = context;
    }
    public async Task<Prescription> CreatePrescriptionAsync(Prescription prescription)
    {
        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
        return prescription;
    }

    public async Task<MedicineInventory> GetMedicineInventoryByIdAsync(int medicineInventoryId)
    {
        return await _context.MedicineInventories
            .Include(mi => mi.Medicine) // Load thông tin Medicine liên quan
            .FirstOrDefaultAsync(mi => mi.MedicineInventoryId == medicineInventoryId);
    }


    public async Task<MedicineConsumption> CreateMedicineConsumptionAsync(MedicineConsumption consumption)
    {
        _context.MedicineConsumptions.Add(consumption);
        await _context.SaveChangesAsync();
        return consumption;
    }

    public async Task<PrescriptionMedicineConsumption> CreatePrescriptionMedicineConsumptionAsync(PrescriptionMedicineConsumption pmc)
    {
        _context.PrescriptionMedicineConsumptions.Add(pmc);
        await _context.SaveChangesAsync();
        return pmc;
    }

   

    public async Task<List<MedicineInventory>> GetAvailableMedicinesAsync()
    {
        var currentDate = DateTime.UtcNow; // Ngày hiện tại
        return await _context.MedicineInventories
            .Include(mi => mi.Medicine)
            .Where(mi => mi.Quantity > 0 && mi.ExpiryDate > currentDate && mi.Medicine.Status == true)
            .ToListAsync();
    }

    public async Task UpdateMedicineInventoryAsync(MedicineInventory inventory)
    {
        _context.MedicineInventories.Update(inventory);
        await _context.SaveChangesAsync();
    }


    public async Task<List<Prescription>> GetPrescriptionsByUserIdAsync(int userId)
    {
        return await _context.Prescriptions
            .Include(p => p.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.IssueDate) 
            .ToListAsync();
    }
    public async Task<List<Prescription>> GetAllPrescriptionsAsync()
    {
        return await _context.Prescriptions
            .Include(p => p.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .OrderByDescending(p => p.IssueDate) 
            .ToListAsync();
    }
    public async Task<List<Prescription>> GetAllPrescriptionsNoBHYTAsync()
    {
        return await _context.Prescriptions
            .Include(p => p.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .OrderByDescending(p => p.IssueDate)
            .ToListAsync();
    }
    public async Task<List<Prescription>> GetPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
    {
        return await _context.Prescriptions
            .Include(p => p.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .Where(p => p.MedicalRecordHistoryId == medicalRecordHistoryId)
            .ToListAsync();
    }
    public async Task<Prescription> GetPrescriptionDetailAsync(int prescriptionId)
    {
        // Bắt đầu từ PrescriptionMedicineConsumption để lấy Prescription và MedicineConsumption
        var prescriptionDetail = await _context.PrescriptionMedicineConsumptions
            .Where(pmc => pmc.PrescriptionId == prescriptionId)
            .Include(pmc => pmc.Prescription)
            .ThenInclude(p => p.User)
            .Include(p => p.Prescription)
            .ThenInclude(p => p.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .Include(pmc => pmc.MedicineConsumtion)
            .ThenInclude(mc => mc.MedicineInventory)
            .ThenInclude(mi => mi.Medicine)
            .Select(pmc => pmc.Prescription) // Lấy Prescription từ kết quả
            .FirstOrDefaultAsync();

        return prescriptionDetail;
    }
    public async Task<Prescription> GetPrescriptionByIdAsync(int prescriptionId)
    {
        return await _context.Prescriptions
            .Include(p => p.MedicalRecordHistory)
            .ThenInclude(mrh => mrh.MedicalRecord)
            .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);
    }

    // Thêm phương thức GetMedicineConsumptionsByPrescriptionIdAsync
    public async Task<List<MedicineConsumption>> GetMedicineConsumptionsByPrescriptionIdAsync(int prescriptionId)
    {
        return await _context.PrescriptionMedicineConsumptions
            .Where(pmc => pmc.PrescriptionId == prescriptionId)
            .Include(pmc => pmc.MedicineConsumtion)
            .Select(pmc => pmc.MedicineConsumtion)
            .ToListAsync();
    }

    // Thêm phương thức UpdatePrescriptionAsync
    public async Task UpdatePrescriptionAsync(Prescription prescription)
    {
        _context.Prescriptions.Update(prescription);
        await _context.SaveChangesAsync();
    }

    // Thêm phương thức GetPrescriptionMedicineConsumptionByConsumptionIdAsync
    public async Task<PrescriptionMedicineConsumption> GetPrescriptionMedicineConsumptionByConsumptionIdAsync(int medicineConsumptionId)
    {
        return await _context.PrescriptionMedicineConsumptions
            .FirstOrDefaultAsync(pmc => pmc.MedicineConsumtionId == medicineConsumptionId);
    }

    // Thêm phương thức DeletePrescriptionMedicineConsumptionAsync
    public async Task DeletePrescriptionMedicineConsumptionAsync(int prescriptionId, int medicineConsumptionId)
    {
        var pmc = await _context.PrescriptionMedicineConsumptions
            .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId && p.MedicineConsumtionId == medicineConsumptionId);
        if (pmc != null)
        {
            _context.PrescriptionMedicineConsumptions.Remove(pmc);
            await _context.SaveChangesAsync();
        }
    }

    // Thêm phương thức DeleteMedicineConsumptionAsync
    public async Task DeleteMedicineConsumptionAsync(int consumptionId)
    {
        var consumption = await _context.MedicineConsumptions.FindAsync(consumptionId);
        if (consumption != null)
        {
            _context.MedicineConsumptions.Remove(consumption);
            await _context.SaveChangesAsync();
        }
    }

    // Thêm phương thức GetMedicineConsumptionByIdAsync
    public async Task<MedicineConsumption> GetMedicineConsumptionByIdAsync(int medicineConsumptionId)
    {
        return await _context.MedicineConsumptions
            .FirstOrDefaultAsync(mc => mc.MedicineConsumptionId == medicineConsumptionId);
    }

    // Thêm phương thức UpdateMedicineConsumptionAsync
    public async Task UpdateMedicineConsumptionAsync(MedicineConsumption consumption)
    {
        _context.MedicineConsumptions.Update(consumption);
        await _context.SaveChangesAsync();
    }

    // Thêm phương thức UpdatePrescriptionMedicineConsumptionAsync
    public async Task UpdatePrescriptionMedicineConsumptionAsync(PrescriptionMedicineConsumption pmc)
    {
        _context.PrescriptionMedicineConsumptions.Update(pmc);
        await _context.SaveChangesAsync();
    }

    public int CountTodayPrescriptions()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return _context.Prescriptions
            .Count(m => m.IssueDate >= today && m.IssueDate < tomorrow);
    }

    public async Task<List<PrescriptionMedicineConsumption>> GetAllMedicineConsumptionsAsync()
    {
        return await _context.PrescriptionMedicineConsumptions
            .Include(pmc => pmc.MedicineConsumtion)
                .ThenInclude(mc => mc.MedicineInventory)
                    .ThenInclude(mi => mi.Medicine)
            .Include(pmc => pmc.Prescription)
            .Where(pmc => pmc.MedicineConsumtion.Status == true)
            .OrderByDescending(pmc => pmc.MedicineConsumtion.ConsumptionDate)
            .ToListAsync();
    }

}
