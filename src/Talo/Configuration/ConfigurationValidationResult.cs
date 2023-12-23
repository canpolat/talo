namespace Talo.Configuration;

public class ConfigurationValidationResult
{
    public bool Success { get; private init; }
    public string? Error { get; private init; }

    public static ConfigurationValidationResult Succeeded() => new() { Success = true, Error = null };

    public static ConfigurationValidationResult Failed(string error) => new() { Success = false, Error = error };
}
