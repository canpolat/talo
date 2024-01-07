using System.CommandLine;
using System.CommandLine.IO;
using System.Text.RegularExpressions;
using Talo.Templating;

namespace Talo.Repositories;

public class RecordMetadata
{
    private readonly IConsole _console;
    public string FilePath { get; }
    public string Title { get; private set; } = "";
    public int SequenceNumber { get; private set; }
    private readonly List<StatusRow> _statusRows = [];
    public string? LatestStatus { get; private set; }

    public RecordMetadata(string filePath, IConsole console)
    {
        _console = console;
        FilePath = filePath;
        Path.GetFileName(filePath);
        ParseFile();
    }

    private static readonly Regex TitleLineRegex =
        new(@"^#\s+([0-9]+)\.\s*(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex StatusHeadingRegex =
        new(@"^##\s+Status\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex StatusTableRowRegex =
        new(@"^\s*\|\s*([^\|]+)\|\s*([^\|]+)\|\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex StatusTableHeaderRegex =
        new(@"^\s*\|[-]+\|[-]+\|\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex NextHeadingRegex =
        new(@"^\s*[#]+.*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public async Task UpdateRecordStatusAsync(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            _console.Error.WriteLine("Provided status value is null or empty");
        }

        await AdjustStatusTableAsync(newStatus);
    }

    public async Task AdjustStatusTableAsync(string? newStatus = null)
    {
        List<string> newLines = [];

        var statusHeadingFound = false;
        var statusTableHeaderFound = false;
        var headingAfterStatusFound = false;
        foreach (var line in File.ReadLines(FilePath))
        {
            if (headingAfterStatusFound)
            {
                newLines.Add(line);
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                if (!statusTableHeaderFound) // should not add the space right after status table
                {
                    newLines.Add(line);
                }

                continue;
            }

            if (!statusHeadingFound && StatusHeadingRegex.IsMatch(line))
            {
                statusHeadingFound = true;
                newLines.Add(line);
                continue;
            }

            if (statusHeadingFound && StatusTableHeaderRegex.IsMatch(line))
            {
                // This is the row that has a line under the table header
                // don't copy
                statusTableHeaderFound = true;
                continue;
            }

            if (statusHeadingFound && !statusTableHeaderFound && StatusTableRowRegex.IsMatch(line))
            {
                // This is the table header row
                // don't copy
                continue;
            }

            if (statusTableHeaderFound && StatusTableRowRegex.IsMatch(line))
            {
                continue;
            }

            if (statusTableHeaderFound && NextHeadingRegex.IsMatch(line))
            {
                headingAfterStatusFound = true;

                // Add status table before next heading
                var table = new MarkdownTable("Status", "Time");
                foreach (var statusRow in _statusRows)
                {
                    table.AddRow(statusRow.Status, statusRow.Time);
                }

                if (!string.IsNullOrWhiteSpace(newStatus))
                {
                    table.AddRow(newStatus, TemplateMap.GetFormattedCurrentTime());
                }

                newLines.Add(table.ToString());

                newLines.Add(line);
                continue;
            }

            newLines.Add(line);
        }

        await WriteLinesToFileAsync(newLines);
    }

    private async Task WriteLinesToFileAsync(IEnumerable<string> lines)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), ".tmp"));
        await File.WriteAllLinesAsync(tempFilePath, lines);
        File.Move(tempFilePath, FilePath, overwrite: true);
        File.Delete(tempFilePath);
    }

    private void ParseFile()
    {
        var titleFound = false;
        var statusHeadingFound = false;
        var statusTableHeaderFound = false;
        foreach (var line in File.ReadLines(FilePath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (!titleFound && TitleLineRegex.IsMatch(line))
            {
                ParseTitleLine(line);
                titleFound = true;
                continue;
            }

            if (!statusHeadingFound && StatusHeadingRegex.IsMatch(line))
            {
                statusHeadingFound = true;
                continue;
            }

            if (statusHeadingFound && StatusTableHeaderRegex.IsMatch(line))
            {
                statusTableHeaderFound = true;
                continue;
            }

            if (statusTableHeaderFound && StatusTableRowRegex.IsMatch(line))
            {
                ParseStatusRow(line);
                continue;
            }

            if (statusTableHeaderFound && NextHeadingRegex.IsMatch(line))
            {
                break;
            }
        }

        if (!titleFound)
        {
            _console.Error.WriteLine($"Title not found in file '{FilePath}'");
            Title = "";
            SequenceNumber = 0;
        }

        LatestStatus = _statusRows.LastOrDefault()?.Status;
    }

    private void ParseTitleLine(string titleLine)
    {
        var match = TitleLineRegex.Match(titleLine);

        Title = match.Groups[2].Value;
        var sequenceNoStr = match.Groups[1].Value;
        if (int.TryParse(sequenceNoStr, out var number))
        {
            SequenceNumber = number;
        }
        else
        {
            _console.Error.WriteLine($"Unable to parse sequence number ({sequenceNoStr}) for file '{FilePath}'");
        }
    }

    private void ParseStatusRow(string line)
    {
        var match = StatusTableRowRegex.Match(line);

        var status = match.Groups[1].Value?.Trim();
        if (string.IsNullOrWhiteSpace(status))
        {
            return;
        }

        var time = match.Groups[2].Value?.Trim();
        var statusRow = new StatusRow(status, time);

        _statusRows.Add(statusRow);
    }

    public static string[] GetTableColumns(bool includeFilepathOptions) =>
        includeFilepathOptions
            ? ["No.", "Status", "Title", "FilePath"]
            : ["No.", "Status", "Title"];

    public string[] AsMarkdownRow(bool includeFilepathOptions) =>
        includeFilepathOptions
            ? [SequenceNumber.ToString(), LatestStatus, Title, FilePath]
            : [SequenceNumber.ToString(), LatestStatus, Title];

    private record StatusRow(string Status, string? Time);
}
