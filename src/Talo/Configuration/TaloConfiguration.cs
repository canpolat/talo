using System.CommandLine;
using System.CommandLine.IO;
using System.Text;
using System.Text.Json;
using Talo.Exceptions;

namespace Talo.Configuration;

public class TaloConfiguration
{
    public AdrConfiguration Adr { get; init; }
    public RfcConfiguration Rfc { get; init; }
    public List<CustomRecordConfiguration> CustomRecordTypes { get; init; }

    public static TaloConfiguration Create(AdrConfiguration adr, RfcConfiguration rfc,
        List<CustomRecordConfiguration> customRecordConfigurations) =>
        new() { Adr = adr, Rfc = rfc, CustomRecordTypes = customRecordConfigurations };

    public static TaloConfiguration Default() =>
        new() { Adr = AdrConfiguration.Default(), Rfc = RfcConfiguration.Default(), CustomRecordTypes = [] };

    public static TaloConfiguration LoadFromFile(FileInfo fileInfo)
    {
        using var r = new StreamReader(fileInfo.FullName);
        string json = r.ReadToEnd();
        var config = JsonSerializer.Deserialize<TaloConfiguration>(json);
        if (config is null)
        {
            throw new InvalidConfigurationException(
                $"Unable to deserialize the configuration file found at {fileInfo.FullName}");
        }

        return config;
    }

    private JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public void Save(DirectoryInfo directoryInfo, IConsole console)
    {
        string jsonString = JsonSerializer.Serialize(this, _jsonSerializerOptions);
        var filePath = Path.Join(directoryInfo.FullName, Constants.ConfigurationFileName);
        File.WriteAllText(filePath, jsonString, Encoding.UTF8);
        console.Out.WriteLine($"Configuration is saved at {directoryInfo.FullName}");
    }

    public IEnumerable<IRecordConfiguration> GetRecordConfigurations()
    {
        yield return Adr;
        yield return Rfc;

        var validationResult = this.Validate();
        if (validationResult.Success)
        {
            foreach (var customRecordType in CustomRecordTypes)
            {
                yield return customRecordType;
            }
        }
    }

    public TaloConfiguration With(IRecordConfiguration recordConfig)
    {
        ArgumentNullException.ThrowIfNull(recordConfig);

        if (recordConfig.Name == Constants.Adr.CommandName)
        {
            var adr = AdrConfiguration.Create(recordConfig);
            return Create(adr, this.Rfc, this.CustomRecordTypes);
        }

        if (recordConfig.Name == Constants.Rfc.CommandName)
        {
            var rfc = RfcConfiguration.Create(recordConfig);
            return Create(this.Adr, rfc, this.CustomRecordTypes);
        }

        var newCustomRecordTypes = new List<CustomRecordConfiguration>();
        foreach (var customRecordType in CustomRecordTypes)
        {
            newCustomRecordTypes.Add(customRecordType.Name == recordConfig.Name
                ? CustomRecordConfiguration.Create(recordConfig)
                : customRecordType);

            return Create(this.Adr, this.Rfc, newCustomRecordTypes);
        }

        return this;
    }

    public TaloConfiguration Add(IRecordConfiguration recordConfig)
    {
        ArgumentNullException.ThrowIfNull(recordConfig);

        CustomRecordTypes.Add(CustomRecordConfiguration.Create(recordConfig));
        return Create(this.Adr, this.Rfc, CustomRecordTypes);
    }

    public ConfigurationValidationResult Validate()
    {
        var allRecordNames = new List<string> { Constants.Adr.CommandName, Constants.Rfc.CommandName };
        foreach (var recordName in CustomRecordTypes.Select(customRecord => customRecord.Name))
        {
            if (string.IsNullOrWhiteSpace(recordName))
            {
                return ConfigurationValidationResult.Failed("Record type cannot be null or empty");
            }

            if (allRecordNames.Contains(recordName))
            {
                return ConfigurationValidationResult.Failed(
                    $"Duplicate record types are not allowed. Offending record name: {recordName}");
            }

            if (recordName.Any(Char.IsWhiteSpace))
            {
                return ConfigurationValidationResult.Failed("Record type cannot contain whitespace");
            }

            if (recordName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return ConfigurationValidationResult.Failed("Record type cannot contain invalid file name characters");
            }

            allRecordNames.Add(recordName);
        }

        return ConfigurationValidationResult.Succeeded();
    }
}
