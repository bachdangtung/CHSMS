using CHSMS.API.DTOs.MedicalRecord;
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

        [HttpGet("Search")]
        public IActionResult GetMedicalRecordHistoriesByDateRange(string? patientName)
        {
            var records = _medicalRecordService.GetAllMedicalRecordsByPatientName(patientName);
            return Ok(records);
        }

        [HttpPost("Add")]
        public IActionResult AddMedicalRecord([FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            try
            {
                var result = _medicalRecordService.AddMedicalRecordHistory(medicalRecordDTO);

                return Ok(new { message = "Thêm bệnh nhân thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
