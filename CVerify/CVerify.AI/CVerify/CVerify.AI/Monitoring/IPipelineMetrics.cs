namespace CVerify.AI.Monitoring;

public interface IPipelineMetrics
{
    void RecordLatency(string stage, long millisecondsElapsed);
    void RecordTokenUsage(string model, int tokenCount);
}
