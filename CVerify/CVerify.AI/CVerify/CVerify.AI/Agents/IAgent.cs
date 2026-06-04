namespace CVerify.AI.Agents;

public interface IAgent
{
    Task<object> ExecuteAsync(object input, CancellationToken cancellationToken = default);
}
