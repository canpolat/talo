using System.CommandLine;
using Talo.Configuration;
using Talo.RecordTypes;
using Talo.Templating;

namespace Talo.Commands;

public class Link(DirectoryInfo taloRootDir)
{
    private const string Description =
        "Links two records to each other and updates statuses accordingly. See command help for usage.";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var linkCommand = new Command(name: "link", description: Description);

        foreach (var recordConfig in recordConfigs)
        {
            var command = new Command(recordConfig.Name, recordConfig.Description);

            var sourceOption = Options.GetSourceOption();
            command.Add(sourceOption);

            var sourceStatusOption = Options.GetSourceStatusOption();
            command.Add(sourceStatusOption);

            var destinationOption = Options.GetDestinationOption();
            command.Add(destinationOption);

            var destinationStatusOption = Options.GetDestinationStatusOption();
            command.Add(destinationStatusOption);

            command.SetHandler(async (context) =>
            {
                var sourceOptionValue = context.ParseResult.GetValueForOption(sourceOption);
                var sourceStatusOptionValue = context.ParseResult.GetValueForOption(sourceStatusOption);
                var destinationOptionValue = context.ParseResult.GetValueForOption(destinationOption);
                var destinationStatusOptionValue = context.ParseResult.GetValueForOption(destinationStatusOption);

                await HandleAsync(recordConfig, sourceOptionValue, sourceStatusOptionValue, destinationOptionValue,
                    destinationStatusOptionValue, context.Console);
            });
            linkCommand.AddCommand(command);
        }

        return linkCommand;
    }

    private async Task HandleAsync(
        IRecordConfiguration recordConfiguration,
        int source,
        string sourceStatus,
        int destination,
        string destinationStatus,
        IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);
        if (!recordConfiguration.IsInitialized())
        {
            throw new InvalidOperationException(
                $"'{recordConfiguration.Name}' is not initialized. Use 'talo init --help' for more information about initialization");
        }
        
        if (source <= 0) throw new ArgumentException("Source number must be positive.");
        if (destination <= 0) throw new ArgumentException("Destination number must be positive.");

        var sourceRecordId = TemplatingEngine.CreateFileId(recordConfiguration.Prefix, source);
        var destinationRecordId = TemplatingEngine.CreateFileId(recordConfiguration.Prefix, destination);

        var recordType = RecordTypeFactory.CreateRecordType(recordConfiguration, taloRootDir, console);
        await recordType.ReviseAction(source, $"{sourceStatus} {destinationRecordId}");
        await recordType.ReviseAction(destination, $"{destinationStatus} {sourceRecordId}");
    }
}
