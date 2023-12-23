namespace Talo.Templating;

public class TemplateMap
{
    public string Type { get; }
    public string Title { get; }
    public int SequenceNumber { get; }
    private readonly Dictionary<string, string> _rest;

    public static string GetFormattedCurrentTime()
    {
        return DateTime.UtcNow.ToString("O");
    }

    private TemplateMap(string type, string title, int sequenceNumber, Dictionary<string, string> rest)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));

        Type = type;
        Title = title;
        SequenceNumber = sequenceNumber;
        _rest = rest ?? throw new ArgumentNullException(nameof(rest));
    }

    public static TemplateMap CreateTemplateMap(string prefix, string title, int sequenceNumber, string status) =>
        new(prefix, title, sequenceNumber,
            new Dictionary<string, string>
            {
                { TemplatingConstants.StatusKey, status },
                { TemplatingConstants.TimeKey, GetFormattedCurrentTime() }
            });

    public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
    {
        yield return new KeyValuePair<string, string>(TemplatingConstants.TitleKey, Title);
        yield return new KeyValuePair<string, string>(TemplatingConstants.SequenceNumberKey, SequenceNumber.ToString());

        foreach (var kvp in _rest)
        {
            yield return kvp;
        }
    }
}
