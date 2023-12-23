using System.CommandLine;

namespace Talo.Commands;

public static class Options
{
    public static Option<string> GetNameOption()
    {
        return GetOption<string>("--name", "Name of the new record type. Use all lower case and no white spaces.", "-n",
            true);
    }

    public static Option<string> GetDescriptionOption()
    {
        return GetOption<string>("--description", "Description of the new record type", "-d");
    }

    public static Option<string> GetLocationOption(bool isRequired = false)
    {
        return GetOption<string>("--location", "Directory location for the record type", "-l", isRequired);
    }

    public static Option<string> GetTemplatePathOption(bool isRequired)
    {
        return GetOption<string>("--template-path",
            "Relative path to the template that will be used when creating new records",
            "-t",
            isRequired);
    }

    public static Option<string> GetPrefixOption()
    {
        return GetOption<string>("--prefix", "Prefix for created files", "-p");
    }

    public static Option<string> GetStatusOption(bool isRequired)
    {
        return GetOption<string>("--status", "Desired status for the new record", "-s", isRequired);
    }

    public static Option<string> GetTitleOption()
    {
        return GetOption<string>("--title", "Title for the new record", "-t", isRequired: true);
    }

    public static Option<int> GetSourceOption()
    {
        return GetOption<int>("--source", "Number of the source record (positive integer)", "-s", isRequired: true);
    }

    public static Option<string> GetSourceStatusOption()
    {
        return GetOption<string>("--source-status",
            "The status text that will be used to update the source record. For example, if source status value 'Amends' is given, the status of the source record will be updated to be 'Amends x' where x is the number of the destination record.",
            "-i", isRequired: true);
    }

    public static Option<int> GetDestinationOption()
    {
        return GetOption<int>("--destination", "Number of the destination record (positive integer)", "-d",
            isRequired: true);
    }

    public static Option<string> GetDestinationStatusOption()
    {
        return GetOption<string>("--destination-status",
            "The status text that will be used to update the destination record. For example, if destination status value 'Amended by' is given, the status of the destination record will be updated to be 'Amended by y' where y is the number of the source record.",
            "-j", isRequired: true);
    }

    public static Option<string> GetFromTemplateOption()
    {
        return GetOption<string>("--from-template",
            "Path of the custom template. If left empty, the template from configuration will be used",
            "-f");
    }

    public static Option<int> GetNumberOption()
    {
        return GetOption<int>("--number", "Sequence number of the revised record (positive integer)", "-n",
            isRequired: true);
    }

    public static Option<int> GetSupersedesOption()
    {
        return GetOption<int>("--supersedes",
            "Sequence number of the record that will be superseded by this record (positive integer)", "-u");
    }

    public static Option<bool> GetIncludeFilepathOption()
    {
        return GetOption<bool>("--include-file-path", "Includes file path in the list", "-f");
    }

    public static Option<bool> GetVerboseOption()
    {
        return GetOption<bool>("--verbose", "Increases log verbosity", "-v");
    }

    public static Option<string> GetOutputOption()
    {
        var outputOption = new Option<string>
            (name: "--output",
            description: "Output directory for the export",
            getDefaultValue: () => "export");
        outputOption.AddAlias("-o");
        return outputOption;
    }

    private static Option<T> GetOption<T>(string name, string description, string alias, bool isRequired = false)
    {
        var nameOption = new Option<T>
            (name: name, description: description) { IsRequired = isRequired };
        nameOption.AddAlias(alias);
        return nameOption;
    }
}
