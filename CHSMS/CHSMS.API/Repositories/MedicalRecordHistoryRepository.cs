using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHSMS.API.Repositories
{
    public class MedicalRecordHistoryRepository : IMedicalRecordHistoryRepository
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
                .OrderByDescending(m => m.Date)
                .ToList();
        }

        public List<MedicalRecordHistory> GetAllTodayMedicalRecordHistories()
        {
            return _context.MedicalRecordHistories
                .Include(m => m.MedicalRecord)
                .Include(m => m.User)
                .Where(m => m.Date == DateTime.Today)
                .OrderByDescending(m => m.Date)
                .ToList();
        }

        public int CountTodayMedicalRecordHistories()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return _context.MedicalRecordHistories
                .Count(m => m.Date >= today && m.Date < tomorrow);
        }

        public MedicalRecordHistory? GetMedicalRecordHistory(int medicalRecordHistoryId)
        {
            return _context.MedicalRecordHistories
            .Include(m => m.MedicalRecord)
            .Include(m => m.User)
            .OrderByDescending(m => m.Date)
            .FirstOrDefault(m => m.MedicalRecordHistoryId == medicalRecordHistoryId);

        }

        public List<MedicalRecordHistory> GetMedicalRecordHistoryByPatientId(int medicalRecordId, DateTime? startDate, DateTime? endDate, string? doctorName)
        {
            var query = _context.MedicalRecordHistories
       .Include(m => m.MedicalRecord)
       .Include(m => m.User)
       .Where(x => x.MedicalRecordId == medicalRecordId)
       .OrderByDescending(m => m.Date)
       .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.Date >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.Date <= endDate);
            }

            if (!string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(x => x.User != null && x.User.Fullname.Contains(doctorName));
            }

            return query.ToList();

        }


        public List<MedicalRecordHistory> GetMedicalRecordHistoriesByFilter(string? doctorName, string? patientName)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var query = _context.MedicalRecordHistories
                .Include(m => m.MedicalRecord)
                .Include(m => m.User)
                .Where(m => m.Date >= today && m.Date < tomorrow)
                .OrderByDescending(m => m.Date)
                .AsQueryable();

            if (!string.IsNullOrEmpty(doctorName))
            {
                query = query.Where(x => x.User != null && x.User.Fullname.Contains(doctorName));
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
            Console.WriteLine($"Thêm MedicalRecordId = {medicalRecordHistory.MedicalRecordId}");
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

        public List<User> GetAllUsers()
        {
            return _context.Users
                .ToList();
        }
    }
}
