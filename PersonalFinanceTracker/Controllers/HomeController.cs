using Microsoft.AspNetCore.Mvc;

namespace PersonalFinanceTracker.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index (or just /)
        public IActionResult Index()
        {
            // This tells the MVC engine to look for the file: Views/Home/Index.cshtml
            return View();
        }

        // GET: /Home/Dashboard (The main view after login)
        public IActionResult Dashboard()
        {
            // This tells the MVC engine to look for the file: Views/Home/Dashboard.cshtml
            return View();
        }
    }
}
