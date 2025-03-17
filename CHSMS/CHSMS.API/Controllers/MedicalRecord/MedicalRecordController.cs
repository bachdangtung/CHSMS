using CHSMS.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.MedicalRecord
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordHistoryService _medicalRecordHistoryService;

        public MedicalRecordController(MedicalRecordHistoryService medicalRecordHistoryService)
        {
            _medicalRecordHistoryService = medicalRecordHistoryService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllMedicalRecords()
        {
            var records = _medicalRecordHistoryService.GetAllMedicalRecordHistories();
            return Ok(records);
        }
    }
}
