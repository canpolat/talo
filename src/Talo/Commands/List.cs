using System.CommandLine;
using System.CommandLine.IO;
using Talo.Configuration;
using Talo.RecordTypes;
using Talo.Templating;

namespace Talo.Commands;

public class List(DirectoryInfo taloRootDir)
{
    private const string Description = "List record types and associated records";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var listCommand = new Command(name: "list", description: Description);

        foreach (var recordConfig in recordConfigs)
        {
            var command = new Command(recordConfig.Name, recordConfig.Description);
            var includeFilepathOption = Options.GetIncludeFilepathOption();
            command.AddOption(includeFilepathOption);

            command.SetHandler(async (context) =>
            {
                var includeFilepathOptionValue = context.ParseResult.GetValueForOption(includeFilepathOption);

                await HandleAsync(recordConfig, includeFilepathOptionValue, context.Console);
            });
            listCommand.AddCommand(command);
        }

        listCommand.SetHandler((context) =>
        {
            var definedRecordConfigs = recordConfigs.Where(x => !string.IsNullOrWhiteSpace(x.Location)).ToList();
            if (definedRecordConfigs.Count == 0)
            {
                context.Console.Out.WriteLine("This talo repository does not contain any record types yet. Use 'init' and/or 'config add' subcommand to register record types.");
                return;
            }

            context.Console.Out.WriteLine("This talo repository contains the following record types:");
            var table = new MarkdownTable("DocumentType", "Description");
            foreach (var recordConfig in definedRecordConfigs)
            {
                table.AddRow(recordConfig.Name, recordConfig.Description);
            }

            context.Console.Out.WriteLine(table.ToString());
        });

        return listCommand;
    }

    private async Task HandleAsync(IRecordConfiguration recordConfiguration, bool includeFilepathOptions, IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);
        if (!recordConfiguration.IsInitialized())
        {
            throw new InvalidOperationException($"'{recordConfiguration.Name}' is not initialized. Use 'talo init --help' for more information about initialization");
        }

        var recordType = RecordTypeFactory.CreateRecordType(recordConfiguration, taloRootDir, console);
        await recordType.ListAction(includeFilepathOptions);
    }
}
