using Microsoft.AspNetCore.Mvc;

namespace TechnorucsWalkInAPI.Controllers
{
    public class RegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
