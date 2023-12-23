using System.CommandLine;
using System.Text.RegularExpressions;

namespace Talo.Repositories;

public class RecordRepository
{
    private readonly string _dirPath;
    private readonly string _prefix;
    private readonly IOrderedEnumerable<RecordFileInfo> _records;
    private IEnumerable<RecordMetadata> _recordsWithMetadata = [];

    public RecordRepository(string dirPath, string prefix)
    {
        _dirPath = dirPath;
        _prefix = prefix;
        _records = GetAllRecords();
    }

    private Regex GetRecordFileNameRegex()
    {
        var pattern = $"^{_prefix}([0-9]{{4}})";
        return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public int GetNextSequenceNumber()
    {
        var lastRecord = _records.MaxBy(x => x.SequenceNumber);
        if (lastRecord is null)
        {
            return 1;
        }

        return lastRecord.SequenceNumber + 1;
    }
    
    public RecordFileInfo GetRecordWithSequenceNumber(int number)
    {
        var record = _records.FirstOrDefault(x => x.SequenceNumber == number);
        if (record is null)
        {
            throw new InvalidOperationException($"Record with sequence number {number} doesn't exist in {_dirPath}");
        }

        return record;
    }
    
    public IEnumerable<RecordMetadata> ParseMetadataFromFiles(IConsole console)
    {
        if (_recordsWithMetadata.Any())
        {
            return _recordsWithMetadata;
        }
        _recordsWithMetadata = _records.Select(r => new RecordMetadata(r.FilePath, console));
        return _recordsWithMetadata;
    }

    private IOrderedEnumerable<RecordFileInfo> GetAllRecords()
    {
        var regex = GetRecordFileNameRegex();
        return Directory.EnumerateFiles(_dirPath)
            .Where(f => regex.IsMatch(Path.GetFileName(f)))
            .Select(AsRecordFile)
            .OrderBy(x => x.SequenceNumber);
    }

    private RecordFileInfo AsRecordFile(string filePath)
    {
        var fileNameRegex = GetRecordFileNameRegex();
        var fileName = Path.GetFileName(filePath);
        var match = fileNameRegex.Match(fileName);

        if (match.Success && int.TryParse(match.Groups[1].Value, out var sequenceNumber))
        {
            return new RecordFileInfo(sequenceNumber, filePath);
        }

        throw new InvalidOperationException("File name doesn't match the defined format");
    }
}
