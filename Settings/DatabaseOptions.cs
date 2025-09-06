namespace jgz_evenemangel.Settings;

public class DatabaseOptions
{
    public const string Section = "Database";
    public required string AttendeesCollectionName { get; init; }
    public required string ConnectionString { get; init; }
    public required string DatabaseName { get; init; }
    public required string EventsCollectionName { get; init; }
}