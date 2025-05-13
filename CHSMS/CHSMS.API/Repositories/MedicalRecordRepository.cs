using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;

namespace CHSMS.API.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly CHSMSContext _context;

        public MedicalRecordRepository(CHSMSContext context)
        {
            _context = context;
        }

        public List<MedicalRecord> GetAllMedicalRecords()
        {
            return _context.MedicalRecords
                .ToList();
        }

        public List<MedicalRecord> GetMedicalRecordsByPatientName(string? patientName, string? healthInsurance)
        {
            var query = _context.MedicalRecords
                .AsQueryable();

            if (!string.IsNullOrEmpty(patientName))
            {
                query = query.Where(p => p.PatientName != null && p.PatientName.Contains(patientName));
            }

            if (!string.IsNullOrEmpty(healthInsurance))
            {
                query = query.Where(p => p.HealthInsurance != null && p.HealthInsurance.Contains(healthInsurance));
            }

            return query.ToList();
        }

        public bool AddMedicalRecordHistory(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            /*
            var sql = _context.Database.GenerateCreateScript();
            Console.WriteLine(sql);
            */
            Console.WriteLine($"Thêm MedicalRecordId = {medicalRecord.MedicalRecordId}");
            return _context.SaveChanges() > 0;
        }
    }
}
