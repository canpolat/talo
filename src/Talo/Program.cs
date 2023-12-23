using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Talo.Commands;
using Talo.FileSystem;

bool logStackTrace = args.Contains("--verbose") || args.Contains("-v");

var fileSearchResult = FileSystemSearcher.SearchConfigurationFile();

var commandBuilder = new CommandBuilder(fileSearchResult);
var rootCommand = commandBuilder.Build();

var commandLineBuilder = new CommandLineBuilder(rootCommand);

commandLineBuilder.AddMiddleware(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        context.Console.Error.WriteLine(ex.Message);
        if (logStackTrace && ex.StackTrace is not null)
        {
            context.Console.Error.WriteLine(ex.StackTrace);
        }
        Console.ResetColor();
        context.ExitCode = 100;
    }
});

commandLineBuilder.UseDefaults();
var parser = commandLineBuilder.Build();
return await parser.InvokeAsync(args);
