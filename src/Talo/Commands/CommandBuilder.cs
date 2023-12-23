using System.CommandLine;
using Talo.Configuration;
using Talo.FileSystem;

namespace Talo.Commands;

public class CommandBuilder(FileSearchResult fileSearchResult)
{
    private const string AppDescription =
        "A CLI tool to manage your ADRs, RFCs, design documents and more. See specific command help for more details about usage.";

    private readonly DirectoryInfo _taloRootDir = fileSearchResult.GetTaloRootDirectory();
    private readonly TaloConfiguration _taloConfiguration = fileSearchResult.GetConfiguration();

    public RootCommand Build()
    {
        var recordConfigs = _taloConfiguration.GetRecordConfigurations().ToList();

        var configCommand = new Config(_taloRootDir, _taloConfiguration).BuildCommand(recordConfigs);
        var initCommand = new Init(_taloRootDir, _taloConfiguration).BuildCommand(recordConfigs);
        var addCommand = new Add(_taloRootDir).BuildCommand(recordConfigs);
        var linkCommand = new Link(_taloRootDir).BuildCommand(recordConfigs);
        var reviseCommand = new Revise(_taloRootDir).BuildCommand(recordConfigs);
        var listCommand = new List(_taloRootDir).BuildCommand(recordConfigs);
        var exportCommand = new Export(_taloRootDir, _taloConfiguration).BuildCommand(recordConfigs);

        var rootCommand = new RootCommand(AppDescription);
        rootCommand.AddCommand(configCommand);
        rootCommand.AddCommand(initCommand);
        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(linkCommand);
        rootCommand.AddCommand(reviseCommand);
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(exportCommand);

        var verboseOption = Options.GetVerboseOption();
        rootCommand.AddGlobalOption(verboseOption);

        return rootCommand;
    }
}
