using System.Text;

namespace Talo.Templating;

public class MarkdownTable
{
    private readonly string[] _columns;
    private int ColumnCount => _columns.Length;
    private readonly List<string?[]> _rows;

    public MarkdownTable(params string[] columns)
    {
        if (columns.Length < 1) throw new InvalidOperationException("Table should contain at least one column");
        _columns = columns;
        _rows = [];
    }
    
    public void AddRow(params string?[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (_columns.Length != values.Length)
            throw new InvalidOperationException($"The number columns must be {_columns.Length}");
        
        _rows.Add(values);
    }

    private IEnumerable<string?[]> GetAllRows()
    {
        yield return _columns;
        foreach (var row in _rows)
        {
            yield return row;
        }
    }
    
    private int[] GetMaxColumnLengths()
    {
        int[] columnLengths = new int[ColumnCount];
        foreach (var row in GetAllRows())
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                var cellLength = row[i]?.Length ?? 0;
                columnLengths[i] = Math.Max(columnLengths[i], cellLength);
            }
        }

        return columnLengths;
    }

    public override string ToString()
    {
        var columnLengths = GetMaxColumnLengths();
        
        var builder = new StringBuilder();
        builder.AppendTableHeader(_columns, columnLengths);

        foreach (var row in _rows)
        {
            builder.AppendTableRow(row, columnLengths);
        }

        return builder.ToString();
    }
}

public static class StringBuilderExtensions
{
    private const int Padding = 1;
    public static void AppendTableRow(this StringBuilder self, string?[] row, IReadOnlyList<int> columnLengths)
    {
        var columnCount = columnLengths.Count;
        for (int i = 0; i < columnCount; i++)
        {
            var cellValue = row[i] ?? ""; 
            var maxColumnLength = columnLengths[i];
            self.Append("| ");
            self.Append(cellValue.PadRight(maxColumnLength + Padding));
        }
        self.Append('|');
        self.Append(Environment.NewLine);
    }
    
    public static void AppendTableHeader(this StringBuilder self, string[] headerRow, IReadOnlyList<int> columnLengths)
    {
        self.AppendTableRow(headerRow, columnLengths);
        
        var columnCount = columnLengths.Count;
        for (int i = 0; i < columnCount; i++)
        {
            var maxColumnLength = columnLengths[i];
            var maxColumnLengthWithPadding = maxColumnLength + (2 * Padding);
            self.Append('|');
            self.Append(new String('-', maxColumnLengthWithPadding));
        }
        self.Append('|');
        self.Append(Environment.NewLine);
    }
}
