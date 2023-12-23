namespace Talo.Configuration;

public class RfcConfiguration : BaseRecordConfiguration
{
    public static RfcConfiguration Create(IRecordConfiguration recordConfiguration) =>
        new()
        {
            Name = Constants.Rfc.CommandName,
            Description = Constants.Rfc.CommandDescription,
            Location = recordConfiguration.Location,
            Prefix = recordConfiguration.Prefix,
            TemplatePath = recordConfiguration.TemplatePath
        };

    public static RfcConfiguration Default() =>
        new() { Name = Constants.Rfc.CommandName, Description = Constants.Rfc.CommandDescription };
}
