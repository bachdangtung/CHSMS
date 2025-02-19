using CHSMS.API.Services;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("GetAll")]
        public IActionResult GetAllMedicalSupplies()
        {
            return Ok(_medicalSupplyService.GetAllMedicalSupplies());
        }
    }
}
