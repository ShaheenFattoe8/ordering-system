using Microsoft.AspNetCore.Mvc;

namespace Fattoe_Shaheen_HW5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}