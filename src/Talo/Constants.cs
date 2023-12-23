namespace Talo;

public static class Constants
{
    public const string ConfigurationFileName = ".talo";

    public static class Adr
    {
        public const string CommandName = "adr";
        public const string CommandDescription = "Architecture Decision Record";
        public const string DefaultStatus = "Accepted";
    }

    public static class Rfc
    {
        public const string CommandName = "rfc";
        public const string CommandDescription = "Request for Comments";
        public const string DefaultStatus = "In Progress";
    }
}
