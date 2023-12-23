using Talo.Repositories;

namespace Talo.RecordTypes;

public interface IRecordType
{
    Task PostInitAction();
    Task<RecordFileInfo> NewAction(string title, string status, string templatePath);
    Task ReviseAction(int number, string newStatus);
    Task ListAction(bool includeFilepathOptions);
}
