using CHSMS.API.DTOs.ExternalPrescription;
using CHSMS.API.Models;
using CHSMS.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.ExternalPrescription
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalPrescriptionController : ControllerBase
    {
        private readonly ExternalPrescriptionService _externalPrescriptionService;
        public ExternalPrescriptionController(ExternalPrescriptionService externalPrescriptionService)
        {
            _externalPrescriptionService = externalPrescriptionService;
        }

        [HttpGet]
        // lấy danh sách thuốc
        [HttpGet("get-all-medicine")]
        public async Task<IActionResult> GetMedicinesForSelectionNoBHYT()
        {
            try
            {
                var medicines = await _externalPrescriptionService.GetMedicinesForExternalPrescriptionAsync();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // tạo đơn thuốc ngoài
        [HttpPost("create-external-prescription/{medicalRecordHistoryId}")]
        public async Task<IActionResult> CreateExternalPrescription(int medicalRecordHistoryId, [FromBody] CreateExternalPrescriptionDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("Id")?.Value);
                var externalPrescriptionId = await _externalPrescriptionService.CreateExternalPrescriptionAsync(userId, medicalRecordHistoryId, dto);
                return Ok(new { Message = "Tạo đơn thuốc thành công!", ExternalPrescriptionId = externalPrescriptionId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("get-external-prescription-by-medical-record/{medicalRecordHistoryId}")]
        public async Task<IActionResult> GetPrescriptionsByMedicalRecordHistoryId(int medicalRecordHistoryId)
        {
            try
            {
                var prescriptions = await _externalPrescriptionService.GetExternalPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
                if (prescriptions == null || !prescriptions.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy đơn thuốc nào" });
                }
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("edit-external-prescription/{externalPrescriptionId}/{medicalRecordHistoryId}")]
        public async Task<IActionResult> EditExternalPrescription(int externalPrescriptionId, int medicalRecordHistoryId, [FromBody] EditExternalPrescriptionDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { Message = "Dữ liệu gửi lên không hợp lệ." });
                if (externalPrescriptionId != dto.ExternalPrescriptionId)
                    return BadRequest(new { Message = "ID trong URL không khớp với ID trong dữ liệu." });

                var userId = int.Parse(User.FindFirst("Id")?.Value);
                dto.ExternalPrescriptionId = externalPrescriptionId;
                dto.MedicalRecordHistoryId = medicalRecordHistoryId;
                dto.UserId = userId;
                await _externalPrescriptionService.EditExternalPrescriptionForDoctorAsync(dto);
                return Ok(new { Message = "Chỉnh sửa đơn thuốc ngoài thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Lỗi khi chỉnh sửa đơn thuốc ngoài: {ex.Message}" });
            }
        }

        
        [HttpGet("external-prescription-detail/{externalPrescriptionId}")]
        public async Task<IActionResult> GetExternalPrescriptionDetail(int externalPrescriptionId)
        {
            try
            {
                if (externalPrescriptionId <= 0)
                    return BadRequest(new { Message = "ExternalPrescriptionId không hợp lệ." });

                var prescription = await _externalPrescriptionService.GetExternalPrescriptionDetailAsync(externalPrescriptionId);
                return Ok(prescription);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Lỗi khi lấy chi tiết đơn thuốc ngoài: {ex.Message}" });
            }
        }

    }
}
