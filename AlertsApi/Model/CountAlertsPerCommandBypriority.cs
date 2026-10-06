namespace AlertsApi.Model;

public class CountAlertsPerCommandBypriority
{
    public Priority North { get; set; } = null!;
    public Priority South { get; set; } = null!;
    public Priority Center { get; set; } = null!;
    public Priority Overseas { get; set; } = null!;
}

public class Priority
{
    public int Low { get; set; }
    public int Medium { get; set; }
    public int High { get; set; }
    public int Critial { get; set; }
}
