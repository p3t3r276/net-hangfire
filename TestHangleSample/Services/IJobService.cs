namespace TestHangleSample.Services;

public interface IJobService
{
    void SendJob(string jobTypeName, string? startTime = "");
}