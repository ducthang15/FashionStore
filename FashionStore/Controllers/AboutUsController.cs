using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
