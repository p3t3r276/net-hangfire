using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using TestHangleSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage("Hangfire.db"));
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IJobService, JobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.MapGet("/start-jobs", (
    [FromServices] ILogger<Program> logger,
    [FromServices] IJobService jobService,
    CancellationToken cancellationToken) => {

    logger.LogInformation("Index-Miminmal API");

    jobService.SendJob("Direct call", DateTime.Now.ToLongTimeString());

    BackgroundJob.Enqueue(() => jobService.SendJob("Fire-and-Forget", DateTime.Now.ToLongTimeString()));

    BackgroundJob.Schedule(() => jobService.SendJob("Delayed Job", DateTime.Now.ToLongTimeString()), TimeSpan.FromSeconds(30));

    RecurringJob.AddOrUpdate("RecurJob", () => jobService.SendJob("RecurringJob", DateTime.Now.ToLongTimeString()), Cron.Minutely);

    var jobId = BackgroundJob.Schedule(() => jobService.SendJob("Continuation Job 1", DateTime.Now.ToLongTimeString()), TimeSpan.FromSeconds(45));
    BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation Job 2 - Email reminder - " + DateTime.Now.ToLongTimeString()));
        
    return TypedResults.Ok("okman");
});

app.Run();
