using muatamer_camunda_poc.Enum;

namespace muatamer_camunda_poc.Models;

public class TotalQuotaTracking
{
    public int Id { get; set; }
    public QuotaType Type { get; set; }
    public int EntityId { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Total { get; set; }
    public int Used { get; set; }
    public int Reserved { get; set; }
}

