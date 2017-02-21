using Microsoft.AspNetCore.Mvc;

namespace SoundIdentifier.Controllers
{
    public class ReportController : Controller
    {

        

        [HttpGet]
        public IActionResult Help()
        {
            // TODO: Describe usage
            return new JsonResult("Help for report");
        }

        [HttpGet]
        public IActionResult GetReport()
        {
            // TODO generate monthly report
            return new JsonResult("Returned report");
        }
    }
}