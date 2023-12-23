using System.CommandLine;
using System.CommandLine.IO;
using System.Text;
using Talo.Configuration;
using Talo.Repositories;
using Talo.Templating;

namespace Talo.RecordTypes;

public class BaseRecordType(
    IRecordConfiguration recordConfiguration,
    FileSystemInfo taloRootDir,
    IConsole console)
    : IRecordType
{
    protected readonly IRecordConfiguration RecordConfiguration = recordConfiguration;
    protected readonly FileSystemInfo TaloRootDir = taloRootDir;
    protected readonly IConsole Console = console;

    protected virtual string DefaultStatus => "Created";

    protected virtual async Task<string> GetTemplateString(string templatePath)
    {
        return !string.IsNullOrWhiteSpace(templatePath)
            ? await TemplatingEngine.GetTemplateStringAsync(TaloRootDir, templatePath)
            : await RecordConfiguration.GetTemplateStringAsync(TaloRootDir);
    }

    public virtual Task PostInitAction() => Task.CompletedTask;

    public async Task<RecordFileInfo> NewAction(string title, string status, string templatePath)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            status = DefaultStatus;
        }

        var dirPath = RecordConfiguration.GetRecordDirectoryPath(TaloRootDir);
        if (!Directory.Exists(dirPath))
        {
            throw new InvalidOperationException(
                $"Record directory ({dirPath}) doesn't exist. Please initialize before adding new records.");
        }

        var prefix = RecordConfiguration.Prefix;
        var recordRepository = new RecordRepository(dirPath, prefix);
        var sequenceNo = recordRepository.GetNextSequenceNumber();

        var map = TemplateMap.CreateTemplateMap(
            prefix: prefix,
            title: title,
            sequenceNumber: sequenceNo,
            status: status);

        var (fileName, content) = TemplatingEngine.GenerateContent(map, await GetTemplateString(templatePath));
        var filePath = Path.Join(TaloRootDir.FullName, RecordConfiguration.Location, fileName);

        Console.Out.WriteLine($"Creating new {RecordConfiguration.Name} at {filePath}");
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);

        var recordMetadata = new RecordMetadata(filePath, Console);
        await recordMetadata.AdjustStatusTableAsync();

        return new RecordFileInfo(sequenceNo, filePath);
    }

    public async Task ReviseAction(int number, string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(newStatus));
        }

        var dirPath = RecordConfiguration.GetRecordDirectoryPath(TaloRootDir);
        if (!Directory.Exists(dirPath))
        {
            throw new InvalidOperationException(
                $"Record directory ({dirPath}) doesn't exist. Please initialize before adding new records.");
        }

        var recordRepository = new RecordRepository(dirPath, RecordConfiguration.Prefix);
        var record = recordRepository.GetRecordWithSequenceNumber(number);
        var recordMetadata = new RecordMetadata(record.FilePath, Console);

        await recordMetadata.UpdateRecordStatusAsync(newStatus);
    }

    public Task ListAction(bool includeFilepathOptions)
    {
        var dirPath = RecordConfiguration.GetRecordDirectoryPath(TaloRootDir);
        if (!Directory.Exists(dirPath))
        {
            throw new InvalidOperationException(
                $"Record directory ({dirPath}) doesn't exist. Please initialize before adding new records.");
        }

        var repository = new RecordRepository(dirPath, RecordConfiguration.Prefix);
        var metadata = repository.ParseMetadataFromFiles(Console);

        var table = new MarkdownTable(RecordMetadata.GetTableColumns(includeFilepathOptions));
        foreach (var recordMetadata in metadata)
        {
            table.AddRow(recordMetadata.AsMarkdownRow(includeFilepathOptions));
        }

        Console.Out.WriteLine(table.ToString());

        return Task.CompletedTask;
    }
}
