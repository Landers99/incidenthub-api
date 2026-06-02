namespace IncidentHub.Api.Data.Seeders;

public class LargeDatasetSeedResult
{
    public int UsersCreated { get; set; }
    public int IncidentsCreated { get; set; }
    public int CommentsCreated { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime FinishedAtUtc { get; set; }
    public long ElapsedMilliseconds { get; set; }
}
