using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CHSMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly MedicineService _medicineService;
        private readonly ILogger<MedicineController> _logger;

        public MedicineController(MedicineService medicineService, ILogger<MedicineController> logger)
        {
            _medicineService = medicineService;
            _logger = logger;
        }

        [HttpGet("medicines")]
        public ActionResult<IEnumerable<MedicineDTO>> GetAllMedicine()
        {
            var medicines = _medicineService.GetAll();
            if (medicines == null || !medicines.Any())
                return NotFound();
            return Ok(medicines);
        }

        [HttpGet("receivers")]
        public IActionResult GetAllReceivers()
        {
            var receivers = _medicineService.GetAllReceivers();
            return Ok(receivers);
        }

        [HttpGet("suppliers")]
        public IActionResult GetAllSuppliers()
        {
            var suppliers = _medicineService.GetAllSuppliers();
            return Ok(suppliers);
        }

        //get medicine inventory by medicineId
        [HttpGet("GetInventory/{medicineId}")]
        public ActionResult<IEnumerable<MedicineInventoryDTO>> GetMedicineInventory(int medicineId)
        {
            var medicineInventories = _medicineService.GetMedicineInventoryByMedicineId(medicineId);
            if (medicineInventories == null || !medicineInventories.Any())
                return NotFound();
            return Ok(medicineInventories);
        }

        //Get one medical supply by ID
        [HttpGet("Get/{id}")]
        public ActionResult<MedicineDTO> GetMedicineDetail(int id)
        {
            var medicine = _medicineService.GetMedicineById(id);
            if (medicine == null)
                return NotFound();

            return Ok(medicine);
        }

        //search medicine by name
        [HttpGet("SearchByName")]
        public ActionResult<IEnumerable<MedicineDTO>> SearchMedicine([FromQuery] string name)
        {
            var medicines = _medicineService.SearchMedicineByName(name);
            if (medicines == null || !medicines.Any())
                return NotFound();
            return Ok(medicines);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<MedicineDTO>>> SearchMedicines(
    [FromQuery] int? medicineId = null,
    [FromQuery] string? medicineName = null,
    [FromQuery] string? activeIngredient = null,
    [FromQuery] string? dosage = null,
    [FromQuery] string? dosageForm = null,
    [FromQuery] double? quantity = null,
    [FromQuery] double? importPrice = null,
    [FromQuery] string? expiryDate = null,
    [FromQuery] string? minExpiryDate = null,
    [FromQuery] string? maxExpiryDate = null,
    [FromQuery] string? batchNumber = null,
    [FromQuery] string? bidNumber = null,
    [FromQuery] bool? status = null)
        {
            // Chuyển đổi expiryDate từ string sang DateTime?
            DateTime? parsedExpiryDate = TryParseDate(expiryDate, "expiryDate");
            DateTime? parsedMinExpiryDate = TryParseDate(minExpiryDate, "minExpiryDate");
            DateTime? parsedMaxExpiryDate = TryParseDate(maxExpiryDate, "maxExpiryDate");

            if (parsedExpiryDate == null && expiryDate != null)
                return BadRequest("Định dạng expiryDate không hợp lệ. Vui lòng dùng dd/MM/yyyy.");

            if (parsedMinExpiryDate == null && minExpiryDate != null)
                return BadRequest("Định dạng minExpiryDate không hợp lệ. Vui lòng dùng dd/MM/yyyy.");

            if (parsedMaxExpiryDate == null && maxExpiryDate != null)
                return BadRequest("Định dạng maxExpiryDate không hợp lệ. Vui lòng dùng dd/MM/yyyy.");

            var result = await _medicineService.SearchMedicinesAsync(
                medicineId, medicineName, activeIngredient, dosage, dosageForm, quantity,
                importPrice, parsedExpiryDate, batchNumber, bidNumber, status, 
                parsedMinExpiryDate, parsedMaxExpiryDate
            );

            if (result == null || result.Count == 0)
            {
                return NotFound(new { Message = "Không tìm thấy thuốc phù hợp" });
            }

            return Ok(result);
        }
        // Hàm chuyển đổi DateTime
        private DateTime? TryParseDate(string? dateStr, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return null;

            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                return parsedDate;

            return null; // Trả về null nếu định dạng sai
        }


        //Add more medicine inventory
        [HttpPost("AddInventoryList")]
        public IActionResult AddMedicineList([FromBody] List<MedicineInventoryAddDTO> medicineList)
        {
            if (medicineList == null || !medicineList.Any())
            {
                _logger.LogWarning("Yêu cầu thêm danh sách thuốc trống.");
                return BadRequest("Danh sách thuốc trống.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dữ liệu DTO không hợp lệ: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Không lấy được userId từ claims.");
                    return Unauthorized("Không xác định được người dùng.");
                }

                var result = _medicineService.AddMedicineInventoryList(medicineList, userId);

                if (!result.IsSuccess && result.Warnings.Any())
                {
                    _logger.LogWarning("Lỗi khi thêm thuốc: {Warnings}", result.Warnings);
                    return BadRequest(new
                    {
                        message = "Có lỗi khi thêm thuốc.",
                        warnings = result.Warnings
                    });
                }

                return Ok(new
                {
                    message = $"Đã thêm {result.AddedCount} thuốc.",
                    warnings = result.Warnings
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi server khi thêm danh sách thuốc.");
                return StatusCode(500, "Đã xảy ra lỗi server. Vui lòng thử lại sau.");
            }
        }



        [HttpPut("UpdateInventory")]
        public IActionResult UpdateInventory([FromBody] MedicineInventoryUpdateDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Không xác định được người dùng.");

                var userId = int.Parse(userIdClaim);

                var success = _medicineService.UpdateMedicineInventory(dto, userId);
                if (success)
                    return Ok("Cập nhật thành công.");
                return BadRequest("Không thể cập nhật bản ghi.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetRecentInventoryHistory")]
        public IActionResult GetRecentInventoryHistory()
        {
            var userIdClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không xác định được người dùng.");

            int userId = int.Parse(userIdClaim);
            var result = _medicineService.GetRecentInventoryHistory(userId);
            return Ok(result);
        }


        [HttpPost("filter-inventory")]
        public ActionResult<List<MedicineStockDTO>> FilterInventory([FromBody] MedicineInventoryFilter filter)
        {
            var result = _medicineService.FilterMedicineStock(filter);
            return Ok(result);
        }
    }
}
