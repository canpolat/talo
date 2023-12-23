using System.CommandLine;
using Talo.Configuration;
using Talo.Templating;

namespace Talo.RecordTypes;

public abstract class BuiltinRecordType(
    IRecordConfiguration recordConfiguration,
    FileSystemInfo taloRootDir,
    IConsole console)
    : BaseRecordType(recordConfiguration, taloRootDir, console)
{
    protected abstract string FileTemplate { get; }

    protected override async Task<string> GetTemplateString(string templatePath)
    {
        if (!string.IsNullOrWhiteSpace(templatePath))
        {
            return await TemplatingEngine.GetTemplateStringAsync(TaloRootDir, templatePath);
        }

        return string.IsNullOrWhiteSpace(RecordConfiguration.TemplatePath)
            ? FileTemplate
            : await RecordConfiguration.GetTemplateStringAsync(TaloRootDir);
    }
}
