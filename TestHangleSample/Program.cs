using Hangfire;
using Hangfire.Storage.SQLite;
using Scalar.AspNetCore;
using TestHangleSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage("Data Source=Hangfire.db;"));
builder.Services.AddHangfireServer();

builder.Services.AddControllers();

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

app.UseEndpoints(endpoints => {
    _ = endpoints.MapControllers();
});

app.Run();
