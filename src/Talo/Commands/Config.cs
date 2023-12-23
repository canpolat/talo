using System.CommandLine;
using Talo.Configuration;

namespace Talo.Commands;

public class Config(DirectoryInfo taloRootDir, TaloConfiguration taloConfiguration)
{
    private const string Description =
        "Configures talo. Define new record types, set directory paths, prefixes and templates.";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var configCommand = new Command("config", Description);

        foreach (var recordConfig in recordConfigs)
        {
            var command = new Command(recordConfig.Name, recordConfig.Description);

            var locationOption = Options.GetLocationOption();
            command.Add(locationOption);

            var templatePathOption = Options.GetTemplatePathOption(isRequired: false);
            command.Add(templatePathOption);

            command.SetHandler((context) =>
            {
                var locationOptionValue = context.ParseResult.GetValueForOption(locationOption);
                var templatePathOptionValue = context.ParseResult.GetValueForOption(templatePathOption);

                HandleUpdate(recordConfig.With(locationOptionValue, templatePathOptionValue), context.Console);
            });

            configCommand.AddCommand(command);
        }

        var configAddCommand = BuildConfigAddCommand(recordConfigs);
        configCommand.AddCommand(configAddCommand);
        return configCommand;

        Command BuildConfigAddCommand(List<IRecordConfiguration> recordConfigs)
        {
            var command = new Command("add", "Add new custom record type");

            var configAddNameOption = Options.GetNameOption();
            command.Add(configAddNameOption);

            var configAddDescriptionOption = Options.GetDescriptionOption();
            command.Add(configAddDescriptionOption);

            var configAddLocationOption = Options.GetLocationOption(isRequired: true);
            command.Add(configAddLocationOption);

            var configAddTemplateOption = Options.GetTemplatePathOption(isRequired: true);
            command.Add(configAddTemplateOption);

            var configAddPrefixOptions = Options.GetPrefixOption();
            command.Add(configAddPrefixOptions);

            command.SetHandler(
                async (context) =>
                {
                    var nameOptionValue = context.ParseResult.GetValueForOption(configAddNameOption);
                    if (!IsValidRecordName(nameOptionValue))
                    {
                        throw new ArgumentException(
                            "Record name must be all lowercase and can only contain letters and digits");
                    }

                    if (recordConfigs.Any(x =>
                            x.Name.Equals(nameOptionValue, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new ArgumentException(
                            $"'{nameOptionValue}' already exists as a record type. 'talo list' displays all registered record types. 'adr' and 'rfc' are built-in types. They can be configured via 'talo init' command.");
                    }

                    var descriptionOptionValue = context.ParseResult.GetValueForOption(configAddDescriptionOption);
                    var locationOptionValue = context.ParseResult.GetValueForOption(configAddLocationOption);
                    var configAddPrefixOptionsValue = context.ParseResult.GetValueForOption(configAddPrefixOptions);
                    var templatePathOptionValue = context.ParseResult.GetValueForOption(configAddTemplateOption);

                    var newRecordConfiguration = new CustomRecordConfiguration
                    {
                        Name = nameOptionValue,
                        Description = descriptionOptionValue,
                        Location = locationOptionValue,
                        Prefix = configAddPrefixOptionsValue,
                        TemplatePath = templatePathOptionValue
                    };

                    HandleAdd(newRecordConfiguration, context.Console);

                    await new Init(taloRootDir, taloConfiguration).HandleAsync(newRecordConfiguration, context.Console);
                });

            return command;
        }
    }

    private void HandleUpdate(IRecordConfiguration recordConfiguration, IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);
        if (!recordConfiguration.IsInitialized())
        {
            throw new InvalidOperationException(
                $"'{recordConfiguration.Name}' is not initialized. Use 'talo init --help' for more information about initialization");
        }

        var newConfig = taloConfiguration.With(recordConfiguration);
        newConfig.Save(taloRootDir, console);
    }

    private void HandleAdd(IRecordConfiguration recordConfiguration, IConsole console)
    {
        ArgumentNullException.ThrowIfNull(recordConfiguration);

        var newConfig = taloConfiguration.Add(recordConfiguration);
        newConfig.Save(taloRootDir, console);
    }

    private static bool IsValidRecordName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.All(c => char.IsLetterOrDigit(c) && char.IsLower(c));
    }
}
