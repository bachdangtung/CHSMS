using CHSMS.API.DTOs.MedicalSupply;
using CHSMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CHSMS.API.Controllers.MedicalSupply
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalSupplyController : ControllerBase
    {
        private readonly IMedicalSupplyService _medicalSupplyService;
        public MedicalSupplyController(IMedicalSupplyService medicalSupplyService)
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
            var medicalSupply = _medicalSupplyService.GetMedicalSupplyById(id.Value);
            if (medicalSupply == null)
                return NotFound();
            return Ok(medicalSupply);
        }

        //Add more medical supplyinventory
        [HttpPost("AddInventoryList")]
        public IActionResult AddMedicalSupply([FromBody] List<MedicalSupplyInventoryDTO> medicalSupplyInventoryDTO)
        {
            var result = _medicalSupplyService.AddMedicalSupplyInventory(medicalSupplyInventoryDTO);
            if (!result)
                return BadRequest();
            return Ok("done");
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
            var result = _medicalSupplyService.ConsumeMedicalSupply(consumpMSDTO);
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
                var expry = _medicalSupplyService.GetExpiryMSI(item.Key.MedicalSupplyId, from, to);
                list.Add(new
                {
                    medicalSupplyId = item.Key.MedicalSupplyId,
                    medicalSupplyName = item.Key.MedicalSupplyName,
                    consump = item.Value,
                    present = item.Key.Quantity.Value,
                    addnew = addOn,
                    expry = expry,
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
                var medicalSupply = _medicalSupplyService.GetMedicalSupplyByMSIId(item.MedicalSupplyInventoryId);
                var medicalSupplyInventory = _medicalSupplyService.GetMedicalSupplyInventoryById(item.MedicalSupplyInventoryId);
                result.Add(new
                {
                    consumpMSID = item.MsconsumptionId,
                    medicalSupplyInventoryId = item.MedicalSupplyInventoryId,
                    medicalSupplyName = medicalSupply.MedicalSupplyName,
                    batchNumber = medicalSupplyInventory.BatchNumber,
                    quantity = item.Amount,
                    date = item.ConsumptionDate,
                    note = item.Note
                });
            }
            return Ok(result);
        }
        [HttpPut("UpdateConsumption")]
        public IActionResult UpdateConsumtion([FromBody] ConsumpMSDTO medicalSupplyConsumption)
        {
            var result = _medicalSupplyService.UpdateMedicalSupplyConsumption(medicalSupplyConsumption);
            if (result == true)
                return Ok();
            return NotFound();

        }

        //Get all medical supply inventory
        [HttpGet("GetMedicalSupplyInventory")]
        public IActionResult GetMedicalSupplyInventory()
        {
            var ms = _medicalSupplyService.GetAllMedicalSupplies();
            List<object> result = new List<object>();
            foreach (var item in ms)
            {
                var medicalSupplyInventory = _medicalSupplyService.GetMedicalSupplyById(item.MedicalSupplyId);
                if (medicalSupplyInventory == null)
                    continue;
                foreach (var item2 in medicalSupplyInventory)
                {
                    if (item2.Quantity == null)
                        continue;
                    result.Add(new
                    {
                        MSID = item.MedicalSupplyId,
                        MSIventoryID = item2.SupplyInventoryId,
                        MSName = item.MedicalSupplyName,
                        MSType = item.SupplyType,
                        BatchNumber = "" + item2.BatchNumber,
                        Quantity = item2.Quantity.Value,
                        UnitOfMeasure = item.UnitOfMeasure.ToString(),
                        ExpiryDate = item2.ExpiryDate.Value,
                        BidNumber = item.BidNumber.Value,
                        TransactionDate = item2.TransactionDate.Value,
                    });
                }
            }
            return Ok(result);
        }

        [HttpGet("GetMedicalSupplyImportHistory")]
        public IActionResult GetMedicalSupplyImportHistory(DateTime fromDate, DateTime toDate)
        {
            var msi = _medicalSupplyService.GetMedicalSupplyImportHistory(fromDate, toDate);
            if (msi == null)
                return NotFound();
            var result = new List<object>();
            foreach (var item in msi)
            {
                var medicalSupply = _medicalSupplyService.GetMedicalSupplyByMSIId(item.MedicalSupplyId);
                result.Add(new
                {
                    MSID = medicalSupply.MedicalSupplyId,
                    MedicalSupplyName = medicalSupply.MedicalSupplyName,
                    CertificateNumber = item.CertificateNumber,
                    BatchNumber = item.BatchNumber,
                    ImportAmount = item.ImportQuantity,
                    TransactionDate = item.TransactionDate,
                    ManufactureDate = item.ManufactureDate,
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
            var list = _medicalSupplyService.GetMedicalSupplyImportHistory(from, to);
            if (list == null)
                return NotFound();
            return Ok(list);
        }
        [HttpGet("GetInventoryStatistic")]
        public IActionResult GetInventoryStatistic(DateTime? from, DateTime? to)
        {
            if (from == null || to == null)
            {
                var result = _medicalSupplyService.GetAllMedicalSupplyInventoryStatistics();
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            var list = _medicalSupplyService.GetMedicalSupplyInventoryStatisticsByStatisticDate(from.Value, to.Value);
            if (list == null)
                return NotFound();
            return Ok(list);
        }
        [HttpPost("AddInventoryStatistic")]
        public IActionResult AddInventoryStatistic([FromBody] List<MSIStatisticDTO> mSIStatisticDTO)
        {
            try
            {
                var result = _medicalSupplyService.AddMedicalSupplyInventoryStatistic(mSIStatisticDTO);
                if (result == false)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateInventoryStatistic")]
        public IActionResult UpdateInventoryStatistic([FromBody] List<MSIStatisticDTO> mSIStatisticDTO)
        {
            var result = _medicalSupplyService.UpdateMedicalSupplyInventoryStatistic(mSIStatisticDTO);
            if (result == false)
                return BadRequest();
            return Ok();
        }
        [HttpDelete("DeleteInventoryStatistic")]
        public IActionResult DeleteInventoryStatistic(int id)
        {
            var result = _medicalSupplyService.DeleteMedicalSupplyInventoryStatistic(id);
            if (result == false)
                return BadRequest();
            return Ok();
        }
    }
}