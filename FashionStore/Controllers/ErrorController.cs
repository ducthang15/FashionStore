using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404;

            return View("404");
        }
    }
}