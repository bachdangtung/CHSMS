using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Services;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("Search")]
        public ActionResult<IEnumerable<MedicineDTO>> SearchMedicine([FromQuery] string name)
        {
            var medicines = _medicineService.SearchMedicineByName(name);
            if (medicines == null || !medicines.Any())
                return NotFound();
            return Ok(medicines);
        }

        //Add more medical supplyinventory
        [HttpPost("AddInventory")]
        public IActionResult AddMedicine([FromBody] MedicineInventoryDTO medicineInventoryDTO)
        {
            if (medicineInventoryDTO == null)
                return BadRequest("Invalid input data.");

            var result = _medicineService.AddMedicineInventory(medicineInventoryDTO);
            if (!result)
                return BadRequest("Failed to add medicine inventory.");

            return Ok("Medicine inventory added successfully.");
        }


        //Update medical supply inventory by MSInventoryID
        [HttpPut("UpdateInventory")]
        public IActionResult UpdateMedicine([FromBody] MedicineInventoryDTO medicineInventoryDTO)
        {
            if (medicineInventoryDTO == null)
                return BadRequest("Invalid input data. Medicine ID is required.");

            var medicineId = medicineInventoryDTO.MedicineId; // Chuyển đổi int? -> int

            var existingMedicine = _medicineService.GetMedicineById(medicineId);
            if (existingMedicine == null)
                return NotFound($"Medicine with ID {medicineId} not found.");

            var result = _medicineService.UpdateMedicineInventory(medicineInventoryDTO);
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
