using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
namespace CHSMS.API.Controllers.MedicalSupply
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalSupplyController : ControllerBase
    {
        private readonly MedicalSupplyService _medicalSupplyService;
        public MedicalSupplyController(MedicalSupplyService medicalSupplyService)
        {
            _medicalSupplyService = medicalSupplyService;
        }

        //Get all medical supplies
        [HttpGet("GetAll")]
        public IActionResult GetAllMedicalSupplies()
        {
            var medicalSupplies = _medicalSupplyService.GetAllMedicalSupplies();
            if (medicalSupplies == null)
                return NotFound();
            return Ok(medicalSupplies);
        }

        //get all medical supply  Actual inventory by date
        [HttpGet("GetQuantity")]
        public IActionResult GetAllMedicalSupplies(DateTime? date)
        {
            var medicalSupplies = _medicalSupplyService.GetAllActualMedicalSupplies(date);
            if (medicalSupplies == null)
                return NotFound();
            return Ok(medicalSupplies);
        }

        //Get one medical supply by ID
        [HttpGet("Get/{id?}")]
        public IActionResult GetMedicalSupplyDetail(int? id)
        {
            if (id == null)
                return GetAllMedicalSupplies();
            var medicalSupply = _medicalSupplyService.GetMedicalSupply(id.Value);
            if (medicalSupply == null)
                return NotFound();
            return Ok(_medicalSupplyService.GetMedicalSupply(id.Value));
        }

        //Add more medical supplyinventory
        [HttpPost("AddInventory")]
        public IActionResult AddMedicalSupply([FromBody] MedicalSupplyInventoryDTO medicalSupplyInventoryDTO)
        {
            var result = _medicalSupplyService.AddMedicalSupplyInventory(medicalSupplyInventoryDTO);
            if (!result)
                return BadRequest();
            return Ok();
        }

        //Update medical supply inventory by MSInventoryID
        [HttpPut("UpdateInventory")]
        public IActionResult UpdateMedicalSupply([FromBody] MedicalSupplyInventoryDTO medicalSupplyInventoryDTO)
        {
            var result = _medicalSupplyService.UpdateMedicalSupplyInventory(medicalSupplyInventoryDTO);
            if (!result)
                return BadRequest();
            return Ok();
        }

        //Medical supply inventory consumption
        [HttpPost("ConsumeInventory")]
        public IActionResult ConsumeMedicalSupply([FromBody] ConsumpMSDTO consumpMSDTO)
        {
            var result = _medicalSupplyService.ConsumeMedicalSupply(consumpMSDTO.MedicalSupplyId, consumpMSDTO.Quantity, consumpMSDTO.BHYT.Value, consumpMSDTO.Note);
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
            var result = _medicalSupplyService.ConsumeReport(from, to);
            foreach (var item in result)
            {
                list.Add(new
                {
                    medicalSupplyId = item.Key.MedicalSupplyId,
                    medicalSupplyName = item.Key.MedicalSupplyName,
                    consump = item.Value
                });
            }
            return Ok(list);
        }
    }
}
