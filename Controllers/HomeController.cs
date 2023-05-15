using Microsoft.AspNetCore.Mvc;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("home")]
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "home", "index.html"), "text/html");
        }
    }
}
