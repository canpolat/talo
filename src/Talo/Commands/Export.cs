using System.CommandLine;
using System.CommandLine.IO;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using Talo.Configuration;
using Talo.Exceptions;
using Talo.FileSystem;
using Talo.Repositories;
using Talo.Templating;

namespace Talo.Commands;

public class Export(FileSystemInfo taloRootDir, TaloConfiguration taloConfiguration)
{
    private const string Description =
        "Exports the records. By default exports all configured record types. See command help for details.";

    public Command BuildCommand(List<IRecordConfiguration> recordConfigs)
    {
        var exportCommand = new Command(name: "export", description: Description);

        var typesOption = new Option<IEnumerable<string>>(
                name: "--types",
                "List of record types to be exported. Multiple values supported.")
            {
                AllowMultipleArgumentsPerToken = true
            }
            .FromAmong(recordConfigs.Select(x => x.Name).ToArray());
        exportCommand.AddOption(typesOption);

        var outputOption = Options.GetOutputOption();
        exportCommand.AddOption(outputOption);

        exportCommand.SetHandler(async (context) =>
        {
            var typesOptionValue = context.ParseResult.GetValueForOption(typesOption);
            var oputputOptionValue = context.ParseResult.GetValueForOption(outputOption);

            await HandleAsync(typesOptionValue.ToList(), oputputOptionValue, context.Console);
        });

        return exportCommand;
    }

    private async Task HandleAsync(List<string> types, string outDir, IConsole console)
    {
        var allConfigurations = taloConfiguration.GetRecordConfigurations().ToList();

        var filteredConfigurations = types.Count == 0
            ? allConfigurations
            : allConfigurations.Where(x => types.Contains(x.Name));

        foreach (var configuration in filteredConfigurations)
        {
            if (!configuration.IsInitialized())
            {
                throw new InvalidOperationException($"'{configuration.Name}' is not initialized. Use 'talo init --help' for more information about initialization");
            }

            var dirPath = configuration.GetRecordDirectoryPath(taloRootDir);
            if (!Directory.Exists(dirPath))
            {
                throw new InvalidConfigurationException(
                    $"Configuration error. Record directory ({dirPath}) doesn't exist.");
            }

            var outputPath = Path.Combine(taloRootDir.FullName, outDir, configuration.Name);
            var outputDir = Directory.CreateDirectory(outputPath);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var repository = new RecordRepository(dirPath, configuration.Prefix);
            var allMetadata = repository.ParseMetadataFromFiles(console);
            List<CreatedHtmlPage> createdFiles = [];
            foreach (var metadata in allMetadata)
            {
                var fileId = await CreateHtmlPage(pipeline, metadata, outputDir, configuration.Prefix);
                createdFiles.Add(new CreatedHtmlPage(
                    SequenceNumber: metadata.SequenceNumber,
                    LatestStatus: metadata.LatestStatus,
                    FileId: fileId,
                    Title: metadata.Title));
            }

            await CreateIndexPage(outputDir, createdFiles, console);
            console.Out.WriteLine($"Record type '{configuration.Name}' is exported to {outputPath}");
        }
    }

    private static async Task CreateIndexPage(FileSystemInfo outputDir, List<CreatedHtmlPage> createdFiles, IConsole console)
    {
        if (createdFiles.Count == 0)
        {
            return;
        }

        var indexFilePath = Path.Combine(outputDir.FullName, "index.html");
        var html = HtmlTemplates.IndexTable(createdFiles);
        console.Out.WriteLine("Creating the index file");
        File.Delete(indexFilePath);
        await File.WriteAllTextAsync(indexFilePath, HtmlTemplates.Top("Index"), Encoding.UTF8);
        await File.AppendAllTextAsync(indexFilePath, html, Encoding.UTF8);
        await File.AppendAllTextAsync(indexFilePath, HtmlTemplates.Bottom, Encoding.UTF8);
    }

    private static async Task<string> CreateHtmlPage(MarkdownPipeline pipeline, RecordMetadata metadata,
        FileSystemInfo outputDir, string prefix)
    {
        var markdownString = await File.ReadAllTextAsync(metadata.FilePath, Encoding.UTF8);
        var modifiedMarkdown = AddInternalLinksToRecords(markdownString, prefix);

        var html = Markdown.ToHtml(modifiedMarkdown, pipeline);

        var fileId = TemplatingEngine.CreateFileId(prefix, metadata.SequenceNumber);
        var htmlFilePath = GetHtmlFilePath(outputDir, fileId);
        File.Delete(htmlFilePath);
        await File.WriteAllTextAsync(htmlFilePath, HtmlTemplates.Top(metadata.Title), Encoding.UTF8);
        await File.AppendAllTextAsync(htmlFilePath, html, Encoding.UTF8);
        await File.AppendAllTextAsync(htmlFilePath, HtmlTemplates.Bottom, Encoding.UTF8);

        return fileId;
    }

    private static string GetHtmlFilePath(FileSystemInfo outputDir, string fileId)
    {
        var htmlFileName = $"{fileId}.html";
        return Path.Combine(outputDir.FullName, htmlFileName);
    }

    private static string AddInternalLinksToRecords(string markdown, string prefix)
    {
        var pattern = $@"\b({prefix}[0-9]{{4}})\b";
        return Regex.Replace(markdown, pattern, "[$1]($1.html)");
    }
}
