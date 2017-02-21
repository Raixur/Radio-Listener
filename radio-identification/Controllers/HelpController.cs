using Microsoft.AspNetCore.Mvc;

namespace SoundIdentifier.Controllers
{
    public class HelpController : Controller
    {
        [HttpGet]
        public IActionResult Help()
        {
            return new JsonResult("Help!");
        }
    }
}