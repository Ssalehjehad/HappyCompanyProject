using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace HappyCompanyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "app_log.txt");
                if (!System.IO.File.Exists(logFilePath))
                {
                    Log.Information("Log file not found on get request");
                    return NotFound("Log file not found.");
                }

                var logs = await System.IO.File.ReadAllTextAsync(logFilePath);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving logs");
                return StatusCode(500, "An error occurred while retrieving the logs.");
            }
        }
    }
}
