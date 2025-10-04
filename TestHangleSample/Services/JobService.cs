namespace TestHangleSample.Services;

public class JobService : IJobService
{
    public void SendJob(string jobTypeName, string? startTime = "")
    {
        Console.WriteLine($"{jobTypeName} - {startTime} - Job ran - {DateTime.Now.ToLongTimeString()}");
    }
}