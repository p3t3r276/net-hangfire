using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TestHangleSample.Services;

namespace TestHangleSample.Controllers;

[Route("[controller]")]
public class HomeController(ILogger<HomeController> logger, IJobService jobService) : Controller
{
    [HttpGet]
    [Route("jobs")]
    public IActionResult Index()
    {
        logger.LogInformation("Index-HomeController");

        jobService.SendJob("Direct call", DateTime.Now.ToLongTimeString());

        BackgroundJob.Enqueue(() => jobService.SendJob("Fire-and-Forget", DateTime.Now.ToLongTimeString()));

        BackgroundJob.Schedule(() => jobService.SendJob("Delayed Job", DateTime.Now.ToLongTimeString()), TimeSpan.FromSeconds(30));

        RecurringJob.AddOrUpdate("RecurJob", () => jobService.SendJob("RecurringJob", DateTime.Now.ToLongTimeString()), Cron.Minutely);

        var jobId = BackgroundJob.Schedule(() => jobService.SendJob("Continuation Job 1", DateTime.Now.ToLongTimeString()), TimeSpan.FromSeconds(45));
        BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation Job 2 - Email reminder - " + DateTime.Now.ToLongTimeString()));

        return Ok();
    }
}