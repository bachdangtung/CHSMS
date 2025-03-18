using CHSMS.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.MedicalRecord
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordService _medicalRecordService;

        public MedicalRecordController(MedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllMedicalRecords()
        {
            var records = _medicalRecordService.GetAllMedicalRecords();
            return Ok(records);
        }
    }
}
