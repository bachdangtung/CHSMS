using Microsoft.AspNetCore.Mvc;

namespace CHSMS.API.Controllers.User
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
