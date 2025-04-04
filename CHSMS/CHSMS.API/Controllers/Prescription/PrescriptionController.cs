using CHSMS.API.DTOs.Prescription;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetPrescriptionsByUserId(int userId)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByUserIdListAsync(userId);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("by-medical-record/{medicalRecordHistoryId}")]
    public async Task<IActionResult> GetPrescriptionByMedicalRecordHistoryId(int medicalRecordHistoryId)
    {
        try
        {
            var prescription = await _prescriptionService.GetPrescriptionByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
            return Ok(prescription);
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
    [HttpPost("create")]
    public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDTO dto)
    {
        try
        {
            var prescriptionId = await _prescriptionService.CreatePrescriptionAsync(dto);
            return Ok(new { PrescriptionId = prescriptionId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // edit prescription có trong inventory

    [HttpPut("doctor")]
    public async Task<IActionResult> EditPrescriptionForDoctor([FromBody] EditPrescriptionForDoctorDTO dto)
    {
        try
        {
            await _prescriptionService.EditPrescriptionForDoctorAsync(dto);
            return Ok(new { Message = "Chỉnh sửa đơn thuốc thành công." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("pharmacist")]
    public async Task<IActionResult> EditPrescriptionForPharmacist([FromBody] EditPrescriptionForPharmacistDTO dto)
    {
        try
        {

            if (dto.PrescriptionId <= 0)
                return BadRequest(new { Message = "PrescriptionId không hợp lệ" });

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

    [HttpPost("create-prescription-no-bhyt")]
    public async Task<IActionResult> CreatePrescriptionNoBHYT([FromBody] CreatePrescriptionNoBHYTDTO dto)
    {
        try
        {
            var prescriptionId = await _prescriptionService.CreatePrescriptionNoBHYTAsync(dto);
            return Ok(new { Message = "Tạo đơn thuốc thành công!", PrescriptionId = prescriptionId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
    [HttpGet("medicines-for-selection-no-bhyt")]
    public async Task<IActionResult> GetMedicinesForSelectionNoBHYT()
    {
        try
        {
            var medicines = await _prescriptionService.GetMedicinesForSelectionNoBHYTAsync();
            return Ok(medicines);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


    [HttpPut("no-bhyt/{id}")]
    public async Task<IActionResult> EditPrescriptionNoBHYT(int id, [FromBody] CreatePrescriptionNoBHYTDTO dto)
    {
        try
        {
            var updatedPrescriptionId = await _prescriptionService.EditPrescriptionNoBHYTAsync(id, dto);
            return Ok(new { Message = "Chỉnh sửa đơn thuốc thành công!", PrescriptionId = updatedPrescriptionId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


}




