using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TestHangleSample.Services;

namespace TestHangleSample.Controllers;

[Route("[controller]")]
public class HomeController(ILogger<HomeController> logger, IJobService jobService) : Controller
{
    public IActionResult Index()
    {
        logger.LogInformation("Index-HomeController");

        jobService.SendJob("Direct call", DateTime.Now.ToLongTimeString());

        BackgroundJob.Enqueue(() => jobService.SendJob("Fire-and-Forget", DateTime.Now.ToLongDateString()));

        BackgroundJob.Schedule(() => jobService.SendJob("Delayed Job", DateTime.Now.ToLongDateString()), TimeSpan.FromSeconds(30));

        RecurringJob.AddOrUpdate("RecurJob", () => jobService.SendJob("RecurringJob", DateTime.Now.ToLongDateString()), Cron.Minutely);

        var jobId = BackgroundJob.Schedule(() => jobService.SendJob("Continuation Job 1", DateTime.Now.ToLongDateString()), TimeSpan.FromSeconds(45));
        BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation Job 2 - Email reminder - " + DateTime.Now.ToLongDateString()));
        
        return Ok();
    }
}