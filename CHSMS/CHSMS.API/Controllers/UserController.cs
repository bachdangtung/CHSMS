using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
