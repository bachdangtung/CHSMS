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
    }
}
