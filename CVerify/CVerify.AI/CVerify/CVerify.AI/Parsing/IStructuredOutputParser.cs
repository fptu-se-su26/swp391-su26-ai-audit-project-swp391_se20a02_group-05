namespace CVerify.AI.Parsing;

public interface IStructuredOutputParser
{
    T Parse<T>(string response) where T : class;
}
