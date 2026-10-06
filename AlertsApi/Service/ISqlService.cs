using AlertsApi.Model;

namespace AlertsApi.Service;

public interface ISqlService
{
    Task<CountAlertsDto> CountAlertsPerCommand();
}
