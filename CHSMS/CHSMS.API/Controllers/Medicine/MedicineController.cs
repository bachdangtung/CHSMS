using CHSMS.API.DTOs.Medicine;
using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Services;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("get-all-medicines-inventory")]
        public IActionResult GetAllMedicine1()
        {
            var medicines = _medicineService.GetAllMedicineInInventory();
            if (medicines == null || !medicines.Any())
                return NotFound();
            return Ok(medicines);
        }


        [HttpGet("get-all-medicines")]
        public IActionResult GetAllMedicine()
        {
            var medicines = _medicineService.GetAllMedicine();
            if (medicines == null || !medicines.Any())
                return NotFound();
            return Ok(medicines);
        }

        //get all medical supply  Actual inventory by date
        [HttpGet("get-quantity")]
        public IActionResult GetAllMedicine(DateTime? date)
        {
            var medicines = _medicineService.GetAllActualMedicines(date);
            if (medicines == null)
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
        [HttpGet("get-medicine-inventory/{medicineId}")]
        public ActionResult<IEnumerable<MedicineInventoryDetailDTO>> GetMedicineInventory(int medicineId)
        {
            var medicineInventories = _medicineService.GetMedicineInventoryByMedicineId(medicineId);
            if (medicineInventories == null || !medicineInventories.Any())
                return NotFound();
            return Ok(medicineInventories);
        }

        //Get one medical supply by ID
        [HttpGet("get-medicine-detail/{id}")]
        public ActionResult<MedicineDTO> GetMedicineDetail(int id)
        {
            var medicine = _medicineService.GetMedicineById(id);
            if (medicine == null)
                return NotFound();

            return Ok(medicine);
        }

        //search medicine by name
        [HttpGet("search-medicine-by-name")]
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

        //Medical supply inventory consumption
        [HttpPost("ConsumeInventory")]
        public IActionResult ConsumeMedicine([FromBody] ConsumeMedicineDTO consumeMedicineDTO)
        {
            var result = _medicineService.ConsumeMedicine(consumeMedicineDTO);
            if (result == -3)
                return Problem("Not enough quantity");
            else if (result == -2)
                return Problem("Invalid quantity");
            else if (result == -1)
                return NotFound("Id not found");
            else if (result == 0)
                return BadRequest();
            else if (result == 1)
                return Ok("Done");
            else return BadRequest();
        }

        //Medical supply inventory consumption report
        [HttpGet("ConsumeReport")]
        public IActionResult ConsumeReport(DateTime? from, DateTime? to)
        {
            List<object> list = new List<object>();
            var actual = _medicineService.GetAllActualMedicines(from);
            var result = _medicineService.ConsumeReport(from, to);
            foreach (var item in result)
            {
                var addOn = _medicineService.GetAddOnMedicineInventory(item.Key.MedicineId, from, to);
                var expry = _medicineService.GetExpiryMedicineInventory(item.Key.MedicineId, from, to);
                list.Add(new
                {
                    medicineId = item.Key.MedicineId,
                    medicineName = item.Key.MedicineName,
                    consume = item.Value,
                    present = item.Key.Quantity.Value,
                    addnew = addOn,
                    expry = expry,
                    before = actual.Find(x => x.MedicineId == item.Key.MedicineId).Quantity.Value
                });
            }
            return Ok(list);
        }

        [HttpGet("ConsumptionDetail")]
        public IActionResult ConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _medicineService.ConsumptionDetail(id, from, to);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("ConsumptionHistory")]
        public IActionResult ConsumptionHistory(DateTime? from, DateTime? to)
        {
            var list = _medicineService.ConsumptionHistory(from, to);
            List<object> result = new List<object>();
            foreach (var item in list)
            {
                var medicine = _medicineService.GetMedicineByMedicineInventoryId(item.MedicineInventoryId);
                var medicineInventory = _medicineService.GetMedicineInventoryById(item.MedicineInventoryId);
                result.Add(new
                {
                    consumeMedicineId = item.MedicineConsumptionId,
                    medicineInventoryId = item.MedicineInventoryId,
                    medicineName = medicine.MedicineName,
                    batchNumber = medicineInventory.BatchNumber,
                    quantity = item.Amount,
                    date = item.ConsumptionDate,
                    note = item.Note
                });
            }
            return Ok(result);
        }
        [HttpPut("UpdateConsumption")]
        public IActionResult UpdateConsumtion([FromBody] ConsumeMedicineDTO medicineConsumption)
        {
            var result = _medicineService.UpdateMedicineConsumption(medicineConsumption);
            if (result == true)
                return Ok();
            return NotFound();

        }

        [HttpGet("GetMedicineImportHistory")]
        public IActionResult GetMedicineImportHistory(DateTime fromDate, DateTime toDate)
        {
            var msi = _medicineService.GetMedicineImportHistory(fromDate, toDate);
            if (msi == null)
                return NotFound();
            var result = new List<object>();
            foreach (var item in msi)
            {
                var medicine = _medicineService.GetMedicineByMedicineInventoryId(item.MedicineId);
                result.Add(new
                {
                    MSID = medicine.MedicineId,
                    MedicineName = medicine.MedicineName,
                    CertificateNumber = item.CertificateNumber,
                    BatchNumber = item.BatchNumber,
                    ImportAmount = item.ImportQuantity,
                    TransactionDate = item.TransactionDate,
                    ManufacturingDate = item.ManufacturingDate,
                    ExpiryDate = item.ExpiryDate,
                    Date = item.TransactionDate,
                    Note = item.Note,

                });
            }
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpGet("ImportHistory")]
        public IActionResult ImportHistory(DateTime from, DateTime to)
        {
            var list = _medicineService.GetMedicineImportHistory(from, to);
            if (list == null)
                return NotFound();
            return Ok(list);
        }
        

        [Authorize]
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

        [Authorize]
        [HttpGet("GetAllInventoryHistory")]
        public IActionResult GetAllInventoryHistory()
        {
            var userIdClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Không xác định được người dùng.");

            int userId = int.Parse(userIdClaim);
            var result = _medicineService.GetAllInventoryHistory(userId);
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
