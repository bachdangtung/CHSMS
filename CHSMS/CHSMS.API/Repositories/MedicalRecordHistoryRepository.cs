using CHSMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class MedicalRecordHistoryRepository
    {
        private readonly SEP_TestContext _context;

        public MedicalRecordHistoryRepository(SEP_TestContext context)
        {
            _context = context;
        }


        public List<MedicalRecordHistory> GetAllMedicalRecordHistories()
        {
            return _context.MedicalRecordHistories
                .Include(m => m.MedicalRecord)
                .Include(m => m.User)
                .ToList();
        }


        public MedicalRecordHistory? GetMedicalRecordHistory(int medicalRecordHistoryId)
        {
            return _context.MedicalRecordHistories
            .Include(m => m.MedicalRecord)
            .Include(m => m.User)
            .FirstOrDefault(m => m.MedicalRecordHistoryId == medicalRecordHistoryId);

        }


        public List<MedicalRecordHistory> GetMedicalRecordHistoriesByFilter(DateTime? startDate, DateTime? endDate, string? doctorName, string? patientName)
        {
            var query = _context.MedicalRecordHistories
                .Include(m => m.MedicalRecord)
                .Include(m => m.User)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(x => x.Date >= startDate && x.Date <= endDate);
            }

            if (!string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(doctorName));
            }

            if (!string.IsNullOrEmpty(patientName))
            {
                query = query.Where(x => x.MedicalRecord != null && x.MedicalRecord.PatientName.Contains(patientName));
            }

            return query.ToList();
        }



        public bool AddMedicalRecordHistory(MedicalRecordHistory medicalRecordHistory)
        {
            _context.MedicalRecordHistories.Add(medicalRecordHistory);
            /*
            var sql = _context.Database.GenerateCreateScript();
            Console.WriteLine(sql);
            */
            return _context.SaveChanges() > 0;
        }


        public bool UpdateMedicalRecordHistory(MedicalRecordHistory medicalRecordHistory)
        {
            _context.MedicalRecordHistories.Update(medicalRecordHistory);
            return _context.SaveChanges() > 0;
        }


        public bool DeleteMedicalRecordHistory(int medicalRecordId)
        {
            var record = _context.MedicalRecordHistories.Find(medicalRecordId);
            if (record == null) return false;
            _context.MedicalRecordHistories.Remove(record);
            return _context.SaveChanges() > 0;
        }
    }
}
