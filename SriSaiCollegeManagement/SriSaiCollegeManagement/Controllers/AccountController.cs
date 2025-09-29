using Microsoft.AspNetCore.Mvc;

namespace SriSaiCollegeManagement.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
