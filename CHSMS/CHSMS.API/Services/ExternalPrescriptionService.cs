using CHSMS.API.DTOs;
using CHSMS.API.DTOs.ExternalPrescription;
using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Services
{
    public class ExternalPrescriptionService
    {
        private readonly ExternalPrescriptionRepository _repository;

        private readonly SEP_TestContext _context;


        public ExternalPrescriptionService(ExternalPrescriptionRepository repository, SEP_TestContext context)

        {
            _repository = repository;
            _context = context;

        }
        // Tạo đơn thuốc kê ngoài(thuốc ko được bhyt chi trả)
        public async Task<int> CreateExternalPrescriptionAsync(int userId, int medicalRecordHistoryId, CreateExternalPrescriptionDTO dto)
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
                var validMedicines = await _repository.GetMedicinesForExternalPrescriptionAsync();

                // Tạo Prescription
                var externalPrescription = new ExternalPrescription
                {
                    MedicalRecordHistoryId = medicalRecordHistoryId,
                    UserId = userId,
                    IssueDate = dto.IssueDate,
                    Status = true, 
                    Note = dto.Note,
                    IsBhyt = false,// Mặc định là false
                };
                var createdExternalPrescription = await _repository.CreateExternalPrescriptionAsync(externalPrescription);

                // Tạo MedicinePrescription
                foreach (var medDto in dto.MedicinesToAdd)
                {
                    // Kiểm tra xem MedicineId có trong danh sách thuốc hợp lệ không
                    var medicine = validMedicines.FirstOrDefault(m => m.MedicineId == medDto.MedicineId);
                    if (medicine == null)
                        throw new Exception($"Không tìm thấy thuốc với ID: {medDto.MedicineId} hoặc thuốc không hoạt động!");

                    var medicinePrescription = new MedicinePrescription
                    {
                        ExternalPrescriptionId = createdExternalPrescription.ExternalPrescriptionId,
                        MedicineId = medDto.MedicineId,
                        Amount = medDto.Amount,
                        Note = medDto.Note
                    };
                    await _repository.CreateExternalMedicinePrescriptionAsync(medicinePrescription);
                }

                await transaction.CommitAsync();
                return createdExternalPrescription.ExternalPrescriptionId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi tạo đơn thuốc kê ngoài: {ex.Message}");
            }
        }

        public async Task<List<Medicine>> GetMedicinesForExternalPrescriptionAsync()
        {
            return await _repository.GetMedicinesForExternalPrescriptionAsync();
        }

        public async Task EditExternalPrescriptionForDoctorAsync(EditExternalPrescriptionDTO dto)
        {
            // Kiểm tra DTO
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Kiểm tra ngày phát hành không được trong tương lai
            if (dto.IssueDate > DateTime.Now)
                throw new Exception("Ngày phát hành không được là ngày trong tương lai!");

            // Kiểm tra tồn tại đơn thuốc
            var prescription = await _repository.GetExternalPrescriptionByIdAsync(dto.ExternalPrescriptionId);
            if (prescription == null)
                throw new Exception($"Không tìm thấy đơn thuốc với ID: {dto.ExternalPrescriptionId}");

            if (prescription.IssueDate != DateTime.Now.Date)
                throw new Exception("Chỉ được chỉnh sửa đơn thuốc trong ngày kê đơn!");

            // Cập nhật thông tin đơn thuốc
            prescription.MedicalRecordHistoryId = dto.MedicalRecordHistoryId;
            prescription.UserId = dto.UserId;
            prescription.IssueDate = dto.IssueDate;
            prescription.Note = dto.Note;
            prescription.IsBhyt = false; // Đơn ngoài luôn không thuộc BHYT
            await _repository.UpdateExternalPrescriptionAsync(prescription);

            // Xóa các Medicine_Prescription được chỉ định
            foreach (var medicineId in dto.MedicinePrescriptionIdsToRemove)
            {
                await _repository.DeleteMedicinePrescriptionAsync(dto.ExternalPrescriptionId, medicineId);
            }

            // Kiểm tra số lượng thuốc trong đơn
            var existingMedicines = await _repository.GetMedicinePrescriptionsByPrescriptionIdAsync(dto.ExternalPrescriptionId);
            if (existingMedicines.Count + dto.MedicinesToAdd.Count > 10)
                throw new Exception("Một đơn thuốc không được chứa quá 10 loại thuốc!");

            // Kiểm tra trùng MedicineId
            var medicineIds = dto.MedicinesToAdd.Select(mc => mc.MedicineId).ToList();
            if (medicineIds.Distinct().Count() != medicineIds.Count)
                throw new Exception("Có thuốc bị trùng trong danh sách thêm mới. Vui lòng kiểm tra lại!");

            // Kiểm tra thuốc hợp lệ và thêm mới
            var validMedicines = await _repository.GetMedicinesForExternalPrescriptionAsync();
            foreach (var medDto in dto.MedicinesToAdd)
            {
                // Kiểm tra thuốc hợp lệ (Status = true, IsBHYT = false)
                var medicine = validMedicines.FirstOrDefault(m => m.MedicineId == medDto.MedicineId);
                if (medicine == null)
                    throw new Exception($"Không tìm thấy thuốc với ID: {medDto.MedicineId} hoặc thuốc không hoạt động!");

                // Kiểm tra Amount hợp lệ
                if (medDto.Amount <= 0)
                    throw new Exception($"Số lượng thuốc ID: {medDto.MedicineId} phải lớn hơn 0!");

                // Tạo Medicine_Prescription mới
                var medicinePrescription = new MedicinePrescription
                {
                    ExternalPrescriptionId = dto.ExternalPrescriptionId,
                    MedicineId = medDto.MedicineId,
                    Amount = medDto.Amount,
                    Note = medDto.Note
                };
                await _repository.CreateExternalMedicinePrescriptionAsync(medicinePrescription);
            }
        }


        public async Task<List<ExternalPrescriptionDTO>> GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(int medicalRecordHistoryId)
        {
            var externalPrescriptions = await _repository.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
            if (externalPrescriptions == null || !externalPrescriptions.Any())
                throw new Exception("Không tìm thấy đơn thuốc");

            return externalPrescriptions.Select(p => new ExternalPrescriptionDTO
            {
                ExternalPrescriptionId = p.ExternalPrescriptionId,
                IssueDate = p.IssueDate ?? DateTime.MinValue,
                Status = p.Status ?? false,
                Note = p.Note ?? string.Empty,
                IsBhyt = p.IsBhyt ?? false,
                PatientName = p.MedicalRecordHistory?.MedicalRecord?.PatientName
            }).ToList();
        }


        public async Task<ExternalPrescriptionDetailDTO> GetExternalPrescriptionDetailAsync(int externalPrescriptionId)
        {
            if (externalPrescriptionId <= 0)
                throw new ArgumentException("ExternalPrescriptionId không hợp lệ.");

            var prescription = await _repository.GetExternalPrescriptionDetailAsync(externalPrescriptionId);
            if (prescription == null)
                throw new Exception("Không tìm thấy đơn thuốc ngoài");

            // Truy vấn riêng Medicine_Prescription
            var medicinePrescriptions = await _context.MedicinePrescriptions
                .Where(mp => mp.ExternalPrescriptionId == externalPrescriptionId)
                .Include(mp => mp.Medicine)
                .ToListAsync();

            return new ExternalPrescriptionDetailDTO
            {
                ExternalPrescriptionId = prescription.ExternalPrescriptionId,
                IssueDate = prescription.IssueDate ?? DateTime.MinValue,
                Status = prescription.Status ?? false,
                Note = prescription.Note ?? string.Empty,
                FullName = prescription.User?.Fullname ?? string.Empty, // Giả định User có Fullname
                PatientName = prescription.MedicalRecordHistory?.MedicalRecord?.PatientName ?? string.Empty,
                Gender = prescription.MedicalRecordHistory?.MedicalRecord?.Gender ?? string.Empty,
                Dob = prescription.MedicalRecordHistory?.MedicalRecord?.Dob ?? DateTime.MinValue,
                Address = prescription.MedicalRecordHistory?.MedicalRecord?.Address ?? string.Empty,
                HealthInsurance = prescription.MedicalRecordHistory?.MedicalRecord?.HealthInsurance ?? string.Empty,
                DiagnoseConclusion = prescription.MedicalRecordHistory?.DiagnoseConclusion ?? string.Empty,
                IsBhyt = prescription.IsBhyt ?? false,
                Medicines = medicinePrescriptions.Select(mp => new MedicinePrescriptionDetailDTO
                {
                    MedicineId = mp.MedicineId,
                    MedicineName = mp.Medicine?.MedicineName ?? string.Empty,
                    DosageForm = mp.Medicine?.DosageForm ?? string.Empty,
                    Amount = mp.Amount ?? 0,
                    Note = mp.Note ?? string.Empty,
                    IsBhyt = mp.Medicine?.IsBhyt ?? false
                }).ToList()
            };
        }

    }
}
