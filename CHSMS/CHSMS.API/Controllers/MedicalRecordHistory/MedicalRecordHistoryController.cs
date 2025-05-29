using CHSMS.API.DTOs.MedicalRecord;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.MedicalRecord
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordHistoryController : ControllerBase
    {
        private readonly IMedicalRecordHistoryService _medicalRecordHistoryService;

        public MedicalRecordHistoryController(IMedicalRecordHistoryService medicalRecordHistoryService)
        {
            _medicalRecordHistoryService = medicalRecordHistoryService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAllMedicalRecordHiatories()
        {
            var userId = User.FindFirst("id")?.Value;
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

        [HttpGet("GetByP/{Pid}")]
        public IActionResult GetMedicalRecordHistoryByPatientId(int Pid, DateTime? startDate, DateTime? endDate, string? doctorName)
        {
            var record = _medicalRecordHistoryService.GetMedicalRecordHistoryByPatientId(Pid, startDate, endDate, doctorName);
            return Ok(record);
        }

        [HttpGet("Search")]
        public IActionResult GetMedicalRecordHistoriesByDateRange(string? doctorName, string? patientName)
        {
            var records = _medicalRecordHistoryService.GetMedicalRecordHistoriesByFilter(doctorName, patientName);
            return Ok(records);
        }


        [HttpPost("Add")]
        public IActionResult AddMedicalRecordHistory([FromBody] MedicalRecordHistoryDTO medicalRecordDTO)
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            Console.WriteLine("Received Medical Record DTO:");
            Console.WriteLine($"PatientId: {medicalRecordDTO.PatientId}");
            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"DiagnoseConclusion: {medicalRecordDTO.DiagnoseConclusion}");

            try
            {

                //var result = await _authService.EditUserProfileAsync(userId, editUserProfileDto);
                var result = _medicalRecordHistoryService.AddMedicalRecordHistory(userId, medicalRecordDTO);

                return Ok(new { message = "Thêm bệnh án thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdateMedicalRecordHistory([FromBody] MedicalRecordHistoryDTO medicalRecordDTO)
        {
            try
            {
                var result = _medicalRecordHistoryService.UpdateMedicalRecordHistory(medicalRecordDTO);
                return Ok(new { message = "Cập nhật lịch sử bệnh án thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /*        [HttpDelete("Delete/{id}")]
                public IActionResult DeleteMedicalRecordHistory(int id)
                {
                    var result = _medicalRecordHistoryService.DeleteMedicalRecordHistory(id);
                    if (!result)
                        return NotFound();
                    return Ok();
                }*/

        [HttpGet("TodayCount")]
        public IActionResult GetTodayMedicalRecordHistoryCount()
        {
            var count = _medicalRecordHistoryService.GetTodayMedicalRecordHistoryCount();
            return Ok(count);
        }

        [HttpGet("GetAllUser")]
        public IActionResult GetAllUsers()
        {
            var records = _medicalRecordHistoryService.GetAllUsers();
            return Ok(records);
        }
    }
}
