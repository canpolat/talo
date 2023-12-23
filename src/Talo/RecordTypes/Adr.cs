using System.CommandLine;
using System.CommandLine.IO;
using System.Text;
using Talo.Configuration;
using Talo.Templating;

namespace Talo.RecordTypes;

public class Adr(
    IRecordConfiguration recordConfiguration,
    FileSystemInfo taloRootDir,
    IConsole console)
    : BuiltinRecordType(recordConfiguration, taloRootDir, console)
{
    protected override string FileTemplate => BuiltinTemplates.AdrTemplate;
    protected override string DefaultStatus => Constants.Adr.DefaultStatus;

    public override async Task PostInitAction()
    {
        var (fileName, content) = TemplatingEngine.GenerateContent(GetInitMap(), InitTemplate);

        var filePath = Path.Join(TaloRootDir.FullName, RecordConfiguration.Location, fileName);

        Console.Out.WriteLine($"Creating initial {RecordConfiguration.Name} file at {filePath}");
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
    }

    private TemplateMap GetInitMap()
    {
        return TemplateMap.CreateTemplateMap(
            prefix: RecordConfiguration.Prefix,
            title: "Record architecture decisions",
            sequenceNumber: 1,
            status: "Accepted");
    }

    private const string InitTemplate = BuiltinTemplates.AdrInitTemplate;
}
