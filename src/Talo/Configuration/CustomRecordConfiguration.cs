namespace Talo.Configuration;

public class CustomRecordConfiguration : BaseRecordConfiguration
{
    public static CustomRecordConfiguration Create(IRecordConfiguration recordConfiguration) =>
        new()
        {
            Name = recordConfiguration.Name,
            Description = recordConfiguration.Description,
            Location = recordConfiguration.Location,
            Prefix = recordConfiguration.Prefix,
            TemplatePath = recordConfiguration.TemplatePath
        };
}
