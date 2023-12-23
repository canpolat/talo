using System.Text;
using Talo.FileSystem;

namespace Talo.Templating;

public static class HtmlTemplates
{
    public static string Top(string title) => 
        $"""
        <!DOCTYPE html>
        <html lang="en">
          <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@picocss/pico@1/css/pico.min.css">
            <title>{title}</title>
          </head>
          <body>
            <main class="container">
        """;
    
    public const string Bottom = 
        """
            </main>
          </body>
        </html>
        """;

    public static string IndexTable(List<CreatedHtmlPage> createdFiles)
    {
        var builder = new StringBuilder();
        builder.Append("<ul>");
        foreach (var file in createdFiles)
        {
            var fileName = $"{file.FileId}.html";
            builder.Append("<li>");
            builder.Append($"<a href='{fileName}'>{file.SequenceNumber}. {file.Title}</a>");
            builder.Append("</li>");
        }

        builder.Append("</ul>");
        return builder.ToString();
    }
}
