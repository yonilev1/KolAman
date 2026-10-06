using AlertsApi.Model;

namespace AlertsApi.Service;

public interface ISqlService
{
    Task<CountAlertsDto> CountAlertsPerCommand();
    Task<CountAlertsPerCommandBypriority> CountAlertsPerCommandBypriority();
    Task<CountAlertsPerCommandByStatus> CountAlertsPerCommandByStatus();
    Task<HotestCommand> GetHotestCommand();
}
