using System.CommandLine;
using Talo.Configuration;

namespace Talo.RecordTypes;

public static class RecordTypeFactory
{
    public static IRecordType CreateRecordType(
        IRecordConfiguration recordConfiguration,
        DirectoryInfo taloRootDir,
        IConsole console)
    {
        return recordConfiguration.Name switch
        {
            Constants.Adr.CommandName => new Adr(recordConfiguration, taloRootDir, console),
            Constants.Rfc.CommandName => new Rfc(recordConfiguration, taloRootDir, console),
            _ => new BaseRecordType(recordConfiguration, taloRootDir, console)
        };
    }
}
