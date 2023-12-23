using System.CommandLine;
using Talo.Configuration;
using Talo.RecordTypes;

namespace Talo.Commands;

public class Revise(DirectoryInfo taloRootDir)
{
    private const string Description = "Updates status of a record";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var reviseCommand = new Command(name: "revise", description: Description);

        foreach (var recordConfig in recordConfigs)
        {
            var command = new Command(recordConfig.Name, recordConfig.Description);

            var numberOption = Options.GetNumberOption();
            command.Add(numberOption);

            var statusOption = Options.GetStatusOption(isRequired: true);
            command.Add(statusOption);

            command.SetHandler(async (context) =>
            {
                var numberOptionValue = context.ParseResult.GetValueForOption(numberOption);
                var statusOptionValue = context.ParseResult.GetValueForOption(statusOption);

                await HandleAsync(recordConfig, numberOptionValue, statusOptionValue, context.Console);
            });
            reviseCommand.AddCommand(command);
        }

        return reviseCommand;
    }

    private async Task HandleAsync(IRecordConfiguration recordConfiguration, int number, string newStatus, IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);
        if (!recordConfiguration.IsInitialized())
        {
            throw new InvalidOperationException($"'{recordConfiguration.Name}' is not initialized. Use 'talo init --help' for more information about initialization");
        }

        var recordType = RecordTypeFactory.CreateRecordType(recordConfiguration, taloRootDir, console);
        await recordType.ReviseAction(number, newStatus);
    }
}
