using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RadioScheduler.Services;

namespace RadioScheduler.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ILogger _logger;
        private readonly SchedulingService _schedulingService;

        public TaskController(ILoggerFactory loggerFactory , SchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
            _logger = loggerFactory.CreateLogger<TaskController>();
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"User: {id} | ip: {ip}");

            var task = _schedulingService.GetTask();
            _logger.LogInformation($"Radio: {task.RadioName}\nStream: {task.Stream}");

            _schedulingService.AssignTask(task, ip);

            return new JsonResult(task);
        }
    }
}
