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

        public MedicineController(MedicineService medicineService)
        {
            _medicineService = medicineService;
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
                importPrice, parsedExpiryDate, batchNumber, bidNumber, status, parsedMinExpiryDate, parsedMaxExpiryDate
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


        //Add more medical supplyinventory
        [HttpPost("AddInventory")]
        public IActionResult AddMedicine([FromBody] MedicineInventoryAddDTO medicineInventoryAddDTO)
        {
            if (medicineInventoryAddDTO == null)
                return BadRequest("Invalid input data.");

            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Không xác định được người dùng.");
                var userId = int.Parse(userIdClaim);

                var result = _medicineService.AddMedicineInventory(medicineInventoryAddDTO, userId);
                if (!result)
                    return BadRequest("Failed to add medicine inventory.");

                return Ok(new { message = "Medicine inventory added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }


        //Update medical supply inventory by MSInventoryID
        [HttpPut("UpdateInventory")]
        public IActionResult UpdateMedicine([FromBody] MedicineInventoryDTO medicineInventoryDTO)
        {
            if (medicineInventoryDTO == null)
                return BadRequest("Invalid input data. Medicine ID is required.");

            var medicineId = medicineInventoryDTO.MedicineId; // Chuyển đổi int? -> int
            var userId = int.Parse(User.FindFirst("Id")?.Value);

            var existingMedicine = _medicineService.GetMedicineInventoryByMedicineId(medicineId);
            if (existingMedicine == null)
                return NotFound($"Medicine with ID {medicineId} not found.");

            var result = _medicineService.UpdateMedicineInventory(medicineInventoryDTO, userId);
            if (!result)
                return BadRequest("Failed to update medicine inventory.");

            return Ok("Medicine inventory updated successfully.");
        }

        [HttpGet("medicines/suggest")]
        public async Task<IActionResult> GetMedicineSuggestions([FromQuery] MedicineSuggestionRequestDTO request)
        {
            // Gọi service để lấy gợi ý thuốc
            var suggestions = await _medicineService.GetMedicineSuggestions(
                request.Query

            );
            return Ok(suggestions);
        }
    }
}
