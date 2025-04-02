using CHSMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class MedicalRecordRepository
    {
        private readonly SEP_TestContext _context;

        public MedicalRecordRepository(SEP_TestContext context)
        {
            _context = context;
        }

        public List<MedicalRecord> GetAllMedicalRecords()
        {
            return _context.MedicalRecords
                .ToList();
        }

        public List<MedicalRecord> GetMedicalRecordsByPatientName(string? patientName)
        {
            var query = _context.MedicalRecords
                .AsQueryable();

            if (!string.IsNullOrEmpty(patientName))
            {
                query = query.Where(p => p.PatientName != null && p.PatientName.Contains(patientName));
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
