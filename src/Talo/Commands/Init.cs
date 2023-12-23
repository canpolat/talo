using System.CommandLine;
using System.CommandLine.IO;
using Talo.Configuration;
using Talo.RecordTypes;

namespace Talo.Commands;

public class Init(DirectoryInfo taloRootDir, TaloConfiguration taloConfiguration)
{
    private const string Description =
        "Initialize talo in current directory. By default, it doesn't initialize any record types. See command help for more information.";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var initCommand = new Command(name: "init", description: Description);

        foreach (var recordConfig in recordConfigs)
        {
            var command = new Command(recordConfig.Name, recordConfig.Description);

            var locationOption = Options.GetLocationOption();
            command.Add(locationOption);

            var templateOption = Options.GetTemplatePathOption(isRequired: false);
            command.Add(templateOption);

            command.SetHandler(async (context) =>
            {
                var locationOptionValue = context.ParseResult.GetValueForOption(locationOption);
                var templateOptionValue = context.ParseResult.GetValueForOption(templateOption);

                await HandleAsync(recordConfig.With(locationOptionValue, templateOptionValue), context.Console);
            });
            initCommand.AddCommand(command);
        }

        initCommand.SetHandler((context) =>
        {
            taloConfiguration.Save(taloRootDir, context.Console);
            context.Console.Out.WriteLine("Initialization complete");
        });

        return initCommand;
    }

    public async Task HandleAsync(IRecordConfiguration recordConfiguration, IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);

        var newRecordConfig = recordConfiguration.GetInitializedConfiguration();
        var newConfig = taloConfiguration.With(newRecordConfig);
        newConfig.Save(taloRootDir, console);
        _ = CreateDirectory(newRecordConfig.Location, console);

        var recordType = RecordTypeFactory.CreateRecordType(newRecordConfig, taloRootDir, console);
        await recordType.PostInitAction();
    }

    private string CreateDirectory(string relativePath, IConsole console)
    {
        var dirPath = Path.Join(taloRootDir.FullName, relativePath);
        if (Directory.Exists(dirPath))
        {
            console.Error.WriteLine($"A directory already exists at {dirPath}");
            return dirPath;
        }

        Directory.CreateDirectory(dirPath);
        console.Out.WriteLine($"{relativePath} directory created at {dirPath}");
        return dirPath;
    }
}
