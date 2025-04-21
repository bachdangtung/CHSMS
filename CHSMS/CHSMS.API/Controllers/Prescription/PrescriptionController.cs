using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


[Route("api/[controller]")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly PrescriptionService _prescriptionService;

    public PrescriptionController(PrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }


    [HttpGet("get-all-medicines-in-inventory")]
    public async Task<IActionResult> GetMedicines()
    {
        var medicines = await _prescriptionService.GetAllMedicinesInInventoryAsync();
        return Ok(medicines);
    }


    [HttpGet("get-prescription-by-medical-record/{medicalRecordHistoryId}")]
    public async Task<IActionResult> GetPrescriptionsByMedicalRecordHistoryId(int medicalRecordHistoryId)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
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


    [HttpGet("all")]
    public async Task<IActionResult> GetAllPrescriptions()
    {
        try
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("all-today")]
    public async Task<IActionResult> GetTodayPrescriptions()
    {
        try
        {
            var prescriptions = await _prescriptionService.GetTodayPrescriptionsAsync();
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }




    [HttpGet("all-nobhyt")]
    public async Task<IActionResult> GetAllPrescriptionsNoBhyt()
    {
        try
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionsNoBHYTAsync();
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("all-today-nobhyt")]
    public async Task<IActionResult> GetTodayPrescriptionsNoBhyt()
    {
        try
        {
            var prescriptions = await _prescriptionService.GetTodayPrescriptionsNoBHYTAsync();
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }



    [HttpGet("detail/{prescriptionId}")]
    public async Task<IActionResult> GetPrescriptionDetail(int prescriptionId)
    {
        try
        {
            var prescriptionDetail = await _prescriptionService.GetPrescriptionDetailAsync(prescriptionId);
            return Ok(prescriptionDetail);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    
    // tạo đơn thuốc có bhyt
    [HttpPost("create/{medicalRecordHistoryId}")]
    public async Task<IActionResult> CreatePrescription(int medicalRecordHistoryId, [FromBody] CreatePrescriptionDTO dto)
    {

        try
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var prescriptionId = await _prescriptionService.CreatePrescriptionAsync(userId, medicalRecordHistoryId, dto);
            return Ok(new { PrescriptionId = prescriptionId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // edit prescription có trong inventory

    [HttpPut("edit-for-doctor/{prescriptionId}/{medicalRecordHistoryId}")]
    public async Task<IActionResult> EditPrescriptionForDoctor(int prescriptionId, int medicalRecordHistoryId, [FromBody] EditPrescriptionForDoctorDTO dto)
    {
        try
        {
            // Lấy UserId từ token
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Gán các giá trị vào DTO
            dto.PrescriptionId = prescriptionId;
            dto.MedicalRecordHistoryId = medicalRecordHistoryId;
            dto.UserId = userId;

            await _prescriptionService.EditPrescriptionForDoctorAsync(dto);
            return Ok(new { Message = "Chỉnh sửa đơn thuốc thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("edit-for-pharmacist/{prescriptionId}")]
    public async Task<IActionResult> EditPrescriptionForPharmacist(int prescriptionId, [FromBody] EditPrescriptionForPharmacistDTO dto)
    {
        try
        {
            if (prescriptionId <= 0)
                return BadRequest(new { Message = "PrescriptionId không hợp lệ" });

            if (dto.PrescriptionId != prescriptionId)
                return BadRequest(new { Message = "PrescriptionId trong DTO không khớp với prescriptionId trong URL" });

            if (dto.MedicineConsumptionStatuses == null || !dto.MedicineConsumptionStatuses.Any())
                return BadRequest(new { Message = "Danh sách MedicineConsumptionStatuses không được rỗng" });

            await _prescriptionService.EditPrescriptionForPharmacistAsync(dto);
            return Ok(new { Message = "Chỉnh sửa trạng thái đơn thuốc thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    

    [HttpGet("TodayCount")]
    public IActionResult GetTodayPrescriptionCount()
    {
        var count = _prescriptionService.GetTodayPrescriptionCount();
        return Ok(count);
    }

    [HttpGet("statistics-medicine-consumption")]
    public async Task<IActionResult> GetAllMedicineConsumptions()
    {
        try
        {
            var medicineConsumptions = await _prescriptionService.GetAllMedicineConsumptionsAsync();
            return Ok(medicineConsumptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


}




