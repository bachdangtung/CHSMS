using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class ExternalPrescriptionRepository : IExternalPrescriptionRepository
    {
        private readonly CHSMSContext _context;

        public ExternalPrescriptionRepository(CHSMSContext context)
        {
            _context = context;
        }
        // Tạo đơn thuốc ngoài(thuốc không có trong bảo hiểm y tế)

        public async Task<ExternalPrescription> CreateExternalPrescriptionAsync(ExternalPrescription externalPrescription)
        {
            _context.ExternalPrescriptions.Add(externalPrescription);
            await _context.SaveChangesAsync();
            return externalPrescription;
        }

        public async Task CreateExternalMedicinePrescriptionAsync(MedicinePrescription medicinePrescription)
        {
            _context.MedicinePrescriptions.Add(medicinePrescription);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Medicine>> GetMedicinesForExternalPrescriptionAsync()
        {
            return await _context.Medicines
                .Where(m => m.Status == true && m.IsBhyt == false) // Chỉ lấy các thuốc có Status = true
                .Select(m => new Medicine
                {
                    MedicineId = m.MedicineId,
                    MedicineName = m.MedicineName,
                    ActiveIngredient = m.ActiveIngredient,
                    Dosage = m.Dosage,
                    DosageForm = m.DosageForm,
                    IsBhyt = m.IsBhyt
                })
                .ToListAsync();
        }

        // Lấy ExternalPrescription theo ID
        public async Task<ExternalPrescription> GetExternalPrescriptionByIdAsync(int prescriptionId)
        {
            return await _context.ExternalPrescriptions
                .FirstOrDefaultAsync(ep => ep.ExternalPrescriptionId == prescriptionId);
        }

        // Cập nhật ExternalPrescription
        public async Task UpdateExternalPrescriptionAsync(ExternalPrescription prescription)
        {
            _context.ExternalPrescriptions.Update(prescription);
            await _context.SaveChangesAsync();
        }

        // Lấy MedicinePrescription theo ExternalPrescriptionId
        public async Task<List<MedicinePrescription>> GetMedicinePrescriptionsByPrescriptionIdAsync(int prescriptionId)
        {
            return await _context.MedicinePrescriptions
                .Where(mp => mp.ExternalPrescriptionId == prescriptionId)
                .ToListAsync();
        }

        // Xóa MedicinePrescription
        public async Task DeleteMedicinePrescriptionAsync(int prescriptionId, int medicineId)
        {
            var medicinePrescription = await _context.MedicinePrescriptions
                .FirstOrDefaultAsync(mp => mp.ExternalPrescriptionId == prescriptionId && mp.MedicineId == medicineId);
            if (medicinePrescription != null)
            {
                _context.MedicinePrescriptions.Remove(medicinePrescription);
                await _context.SaveChangesAsync();
            }
        }

        // lấy danh sách 
        public async Task<List<ExternalPrescription>> GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
        {
            return await _context.ExternalPrescriptions
                .Include(ep => ep.MedicalRecordHistory)
                .ThenInclude(mrh => mrh.MedicalRecord)
                .Where(ep => ep.MedicalRecordHistoryId == medicalRecordHistoryId)
                .ToListAsync();
        }

        public async Task<ExternalPrescription?> GetExternalPrescriptionDetailAsync(int externalPrescriptionId)
        {
            // Bắt đầu từ Medicine_Prescription để lấy ExternalPrescription và các thông tin liên quan
            var externalPrescription = await _context.MedicinePrescriptions
                .Where(mp => mp.ExternalPrescriptionId == externalPrescriptionId)
                .Include(mp => mp.ExternalPrescription)
                .ThenInclude(ep => ep!.User)
                .Include(mp => mp.ExternalPrescription)
                .ThenInclude(ep => ep!.MedicalRecordHistory)
                .ThenInclude(mrh => mrh!.MedicalRecord)
                .Include(mp => mp.Medicine)
                .Select(mp => mp.ExternalPrescription)
                .FirstOrDefaultAsync();

            return externalPrescription;
        }

    }
}
