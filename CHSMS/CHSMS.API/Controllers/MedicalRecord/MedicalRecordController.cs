using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Services;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.MedicalRecord
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
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
        public IActionResult GetMedicalRecordHistoriesByDateRange(string? patientName, string? healthInsurance)
        {
            var records = _medicalRecordService.GetAllMedicalRecordsByPatientName(patientName, healthInsurance);
            return Ok(records);
        }

        [HttpGet("Get/{id}")]
        public IActionResult GetMedicalRecord(int id)
        {
            var record = _medicalRecordService.GetMedicalRecord(id);
            if (record == null)
                return NotFound();
            return Ok(record);
        }

        [HttpPost("Add")]
        public IActionResult AddMedicalRecord([FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            try
            {
                var result = _medicalRecordService.AddMedicalRecord(medicalRecordDTO);

                return Ok(new { message = "Thêm bệnh nhân thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdateMedicalRecord([FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            try
            {
                var result = _medicalRecordService.UpdateMedicalRecord(medicalRecordDTO);
                return Ok(new { message = "Cập nhật bệnh án thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
