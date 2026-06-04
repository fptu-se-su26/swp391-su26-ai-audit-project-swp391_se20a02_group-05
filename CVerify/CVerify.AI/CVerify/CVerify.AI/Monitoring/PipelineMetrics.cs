namespace CVerify.AI.Monitoring;

public interface IPipelineMetrics
{
    void RecordLatency(string stage, long millisecondsElapsed);
    void RecordTokenUsage(string model, int tokenCount);
}

public class PipelineMetrics : IPipelineMetrics
{
    public void RecordLatency(string stage, long millisecondsElapsed)
    {
        // Implementation will go here - collect metrics
    }

    public void RecordTokenUsage(string model, int tokenCount)
    {
        // Implementation will go here
    }
}
