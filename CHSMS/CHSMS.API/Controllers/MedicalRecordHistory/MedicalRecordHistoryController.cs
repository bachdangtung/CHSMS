using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.MedicalRecord
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordHistoryController : ControllerBase
    {
        private readonly MedicalRecordHistoryService _medicalRecordHistoryService;

        public MedicalRecordHistoryController(MedicalRecordHistoryService medicalRecordHistoryService)
        {
            _medicalRecordHistoryService = medicalRecordHistoryService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAllMedicalRecordHiatories()
        {
            var records = _medicalRecordHistoryService.GetAllMedicalRecordHistories();
            return Ok(records);
        }

        [HttpGet("Get/{id}")]
        public IActionResult GetMedicalRecordHistory(int id)
        {
            var record = _medicalRecordHistoryService.GetMedicalRecordHistory(id);
            if (record == null)
                return NotFound();
            return Ok(record);
        }

        [HttpGet("Search")]
        public IActionResult GetMedicalRecordHistoriesByDateRange(DateTime? startDate, DateTime? endDate, string? doctorName, string? patientName)
        {
            var records = _medicalRecordHistoryService.GetMedicalRecordHistoriesByFilter(startDate, endDate, doctorName, patientName);
            return Ok(records);
        }


        [HttpPost("Add")]
        public IActionResult AddMedicalRecordHistory([FromBody] MedicalRecordHistoryDTO medicalRecordDTO)
        {

            var result = _medicalRecordHistoryService.AddMedicalRecordHistory(medicalRecordDTO);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult UpdateMedicalRecordHistory([FromBody] MedicalRecordHistoryDTO medicalRecordDTO)
        {
            var result = _medicalRecordHistoryService.UpdateMedicalRecordHistory(medicalRecordDTO);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteMedicalRecordHistory(int id)
        {
            var result = _medicalRecordHistoryService.DeleteMedicalRecordHistory(id);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpGet("GetAllUser")]
        public IActionResult GetAllUsers()
        {
            var records = _medicalRecordHistoryService.GetAllUsers();
            return Ok(records);
        }
    }
}
