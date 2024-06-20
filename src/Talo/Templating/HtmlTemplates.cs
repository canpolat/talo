using System.Text;
using Markdig.Helpers;
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
          <script type="module">
            import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
            mermaid.initialize({ startOnLoad: true });
          </script>
          </body>
        </html>
        """;

    public static string IndexTable(List<CreatedHtmlPage> createdFiles)
    {
        var builder = new StringBuilder();
        builder.Append("<table><thead><tr><th>No.</th><th>Status</th><th>Title</th></tr></thead><tbody>");
        foreach (var file in createdFiles)
        {
            var fileName = $"{file.FileId}.html";
            builder.Append("<tr>");
            builder.Append("<td>");
            builder.Append(file.SequenceNumber);
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append(file.LatestStatus);
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append($"<a href='{fileName}'>{file.Title}</a>");
            builder.Append("</td>");
            builder.Append("</tr>");
        }

        builder.Append("</tbody></table>");
        return builder.ToString();
    }
}
