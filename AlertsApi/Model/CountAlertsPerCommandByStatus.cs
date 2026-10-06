namespace AlertsApi.Model;

public class CountAlertsPerCommandByStatus
{
    public Status North { get; set; } = null!;
    public Status South { get; set; } = null!;
    public Status Center { get; set; } = null!;
    public Status Overseas { get; set; } = null!;
}

public class Status
{
    public int Waiting { get; set; }
    public int InPrograss { get; set; }
    public int Canceled { get; set; }
    public int Done { get; set; }
}
