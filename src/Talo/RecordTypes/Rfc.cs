using System.CommandLine;
using Talo.Configuration;
using Talo.Templating;

namespace Talo.RecordTypes;

public class Rfc(
    IRecordConfiguration recordConfiguration,
    FileSystemInfo taloRootDir,
    IConsole console) : BuiltinRecordType(recordConfiguration, taloRootDir, console)
{
    protected override string FileTemplate => BuiltinTemplates.RfcTemplate;
    protected override string DefaultStatus => Constants.Rfc.DefaultStatus;
}
