namespace CVerify.AI.Parsing;

public interface IStructuredOutputParser
{
    T Parse<T>(string response) where T : class;
}

public class JsonSchemaValidator : IStructuredOutputParser
{
    public T Parse<T>(string response) where T : class
    {
        // Implementation will go here - validate and parse JSON
        return default;
    }
}
