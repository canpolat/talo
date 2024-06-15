using System.CommandLine;
using Talo.Configuration;
using Talo.RecordTypes;
using Talo.Templating;

namespace Talo.Commands;

public class Add(DirectoryInfo taloRootDir)
{
    private const string Description =
        "Add a new record of given type. Will create a new record from template and add it to the appropriate directory.";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var addCommand = new Command(name: "new", description: Description);
        addCommand.AddAlias("add");
        addCommand.AddAlias("create");

        foreach (var recordConfig in recordConfigs)
        {
            var command = new Command(recordConfig.Name, recordConfig.Description);

            var titleOption = Options.GetTitleOption();
            command.Add(titleOption);

            var statusOption = Options.GetStatusOption(isRequired: false);
            command.Add(statusOption);

            var supersedesOption = Options.GetSupersedesOption();
            command.Add(supersedesOption);

            var fromTemplateOption = Options.GetFromTemplateOption();
            command.Add(fromTemplateOption);

            command.SetHandler(
                async (context) =>
                {
                    var titleOptionValue = context.ParseResult.GetValueForOption(titleOption);
                    var statusOptionValue = context.ParseResult.GetValueForOption(statusOption);
                    var supersedesOptionValue = context.ParseResult.GetValueForOption(supersedesOption);
                    var fromTemplateOptionValue = context.ParseResult.GetValueForOption(fromTemplateOption);

                    await HandleAsync(
                        recordConfig,
                        titleOptionValue,
                        statusOptionValue,
                        supersedesOptionValue,
                        fromTemplateOptionValue,
                        context.Console);
                });
            addCommand.AddCommand(command);
        }

        return addCommand;
    }

    private async Task HandleAsync(
        IRecordConfiguration recordConfiguration,
        string title,
        string status,
        int supersedes,
        string fromTemplate,
        IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);
        if (!recordConfiguration.IsInitialized())
        {
            throw new InvalidOperationException($"'{recordConfiguration.Name}' is not initialized. Use 'talo init --help' for more information about initialization");
        }

        var recordType = RecordTypeFactory.CreateRecordType(recordConfiguration, taloRootDir, console);
        if (supersedes <= 0)
        {
            _ = await recordType.NewAction(title, status, fromTemplate);
            return;
        }

        var supersededRecordId = TemplatingEngine.CreateFileId(recordConfiguration.Prefix, supersedes);
        var supersedingRecord = await recordType.NewAction(title, $"Supersedes {supersededRecordId}", fromTemplate);

        var supersedingRecordId =
            TemplatingEngine.CreateFileId(recordConfiguration.Prefix, supersedingRecord.SequenceNumber);
        await recordType.ReviseAction(supersedes, $"Superseded by {supersedingRecordId}");
        if (!string.IsNullOrWhiteSpace(status))
        {
            await recordType.ReviseAction(supersedingRecord.SequenceNumber, status);
        }
    }
}
