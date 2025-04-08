using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
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
            var result = _medicalSupplyService.ConsumeMedicalSupply(consumpMSDTO.MedicalSupplyInventoryId.Value, consumpMSDTO.Quantity.Value, consumpMSDTO.Note);
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
            var actual = _medicalSupplyService.GetAllActualMedicalSupplies(from);
            var result = _medicalSupplyService.ConsumeReport(from, to);
            foreach (var item in result)
            {
                var addOn = _medicalSupplyService.GetAddOnMSI(item.Key.MedicalSupplyId, from, to);
                list.Add(new
                {
                    medicalSupplyId = item.Key.MedicalSupplyId,
                    medicalSupplyName = item.Key.MedicalSupplyName,
                    consump = item.Value,
                    present = item.Key.Quantity.Value,
                    addnew = addOn,
                    before = actual.Find(x => x.MedicalSupplyId == item.Key.MedicalSupplyId).Quantity.Value
                });
            }
            return Ok(list);
        }
        [HttpGet("ConsumptionDetail")]
        public IActionResult ConsumptionDetail(int id, DateTime? from, DateTime? to)
        {
            var result = _medicalSupplyService.ConsumptionDetail(id, from, to);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpGet("ConsumptionHistory")]
        public IActionResult ConsumptionHistory(DateTime? from, DateTime? to)
        {
            var list = _medicalSupplyService.ConsumptionHistory(from, to);
            List<object> result = new List<object>();
            foreach (var item in list)
            {
                var medicalSupply = _medicalSupplyService.GetMedicalSupplyByMSIId(item.MedicalSupplyInventoryId.Value);
                result.Add(new
                {
                    medicalSupplyName = medicalSupply.MedicalSupplyName,
                    quantity = item.Amount,
                    date = item.ConsumptionDate,
                    note = item.Note
                });
            }
            return Ok(result);
        }
        [HttpPut("UpdateConsumtion")]
        public IActionResult UpdateConsumtion([FromBody] ConsumpMSDTO medicalSupplyConsumption)
        {
            var result = _medicalSupplyService.UpdateMedicalSupplyConsumption(medicalSupplyConsumption);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
