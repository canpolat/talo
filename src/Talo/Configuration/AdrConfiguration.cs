namespace Talo.Configuration;

public class AdrConfiguration : BaseRecordConfiguration
{
    public static AdrConfiguration Create(IRecordConfiguration recordConfiguration) =>
        new()
        {
            Name = Constants.Adr.CommandName,
            Description = Constants.Adr.CommandDescription,
            Location = recordConfiguration.Location,
            Prefix = recordConfiguration.Prefix,
            TemplatePath = recordConfiguration.TemplatePath
        };

    public static AdrConfiguration Default() =>
        new() { Name = Constants.Adr.CommandName, Description = Constants.Adr.CommandDescription };
}
