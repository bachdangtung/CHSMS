using CHSMS.API.DTOs.UseMedicalSupply;
using CHSMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UseMedicalSupplyController : ControllerBase
{
    private readonly UseMedicalSupplyService _useMedicalSupplyService;

    public UseMedicalSupplyController(UseMedicalSupplyService useMedicalSupplyService)
    {
        _useMedicalSupplyService = useMedicalSupplyService;
    }

    [HttpGet("get-all-medical-supplies-in-inventory")]
    public async Task<IActionResult> GetMedicalSupplies()
    {
        var medicalSupplies = await _useMedicalSupplyService.GetAllMedicalSuppliesInInventoryAsync();
        return Ok(medicalSupplies);
    }

    [HttpGet("get-use-medical-supply-by-medical-record/{medicalRecordHistoryId}")]
    public async Task<IActionResult> GetUseMedicalSuppliesByMedicalRecordHistoryId(int medicalRecordHistoryId)
    {
        try
        {
            var useMedicalSupplies = await _useMedicalSupplyService.GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(medicalRecordHistoryId);
            if (useMedicalSupplies == null || !useMedicalSupplies.Any())
            {
                return NotFound(new { Message = "Không tìm thấy đơn vật tư nào" });
            }
            return Ok(useMedicalSupplies);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUseMedicalSupplies()
    {
        try
        {
            var useMedicalSupplies = await _useMedicalSupplyService.GetAllUseMedicalSuppliesAsync();
            return Ok(useMedicalSupplies);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("detail/{useMedicalSupplyId}")]
    public async Task<IActionResult> GetUseMedicalSupplyDetail(int useMedicalSupplyId)
    {
        try
        {
            var useMedicalSupplyDetail = await _useMedicalSupplyService.GetUseMedicalSupplyDetailAsync(useMedicalSupplyId);
            return Ok(useMedicalSupplyDetail);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("create/{medicalRecordHistoryId}")]
    public async Task<IActionResult> CreateUseMedicalSupply(int medicalRecordHistoryId, [FromBody] CreateUseMedicalSupplyDTO dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("Id")?.Value);
            var useMedicalSupplyId = await _useMedicalSupplyService.CreateUseMedicalSupplyAsync(userId, medicalRecordHistoryId, dto);
            return Ok(new { UseMedicalSupplyId = useMedicalSupplyId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("edit-for-doctor/{useMedicalSupplyId}/{medicalRecordHistoryId}")]
    public async Task<IActionResult> EditUseMedicalSupplyForDoctor(int useMedicalSupplyId, int medicalRecordHistoryId, [FromBody] EditUseMedicalSupplyForDoctorDTO dto)
    {
        try
        {
            // Lấy UserId từ token
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            // Gán các giá trị vào DTO
            dto.UseMedicalSupplyId = useMedicalSupplyId;
            dto.MedicalRecordHistoryId = medicalRecordHistoryId;
            dto.UserId = userId;

            await _useMedicalSupplyService.EditUseMedicalSupplyForDoctorAsync(dto);
            return Ok(new { Message = "Chỉnh sửa đơn vật tư thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("edit-for-pharmacist/{useMedicalSupplyId}")]
    public async Task<IActionResult> EditUseMedicalSupplyForPharmacist(int useMedicalSupplyId, [FromBody] EditUseMedicalSupplyForPharmacistDTO dto)
    {
        try
        {
            if (useMedicalSupplyId <= 0)
                return BadRequest(new { Message = "UseMedicalSupplyId không hợp lệ" });

            if (dto.UseMedicalSupplyId != useMedicalSupplyId)
                return BadRequest(new { Message = "UseMedicalSupplyId trong DTO không khớp với useMedicalSupplyId trong URL" });

            if (dto.MedicalSupplyConsumptionStatuses == null || !dto.MedicalSupplyConsumptionStatuses.Any())
                return BadRequest(new { Message = "Danh sách MedicalSupplyConsumptionStatuses không được rỗng" });

            await _useMedicalSupplyService.EditUseMedicalSupplyForPharmacistAsync(dto);
            return Ok(new { Message = "Chỉnh sửa trạng thái đơn vật tư thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}