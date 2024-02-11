using muatamer_camunda_poc.Enum;

namespace muatamer_camunda_poc.Models;

public class IntersectionQuotaTracking
{
    public int Id { get; set; }
    public QuotaType Entity1Type { get; set; }
    public int Entity1Id { get; set; }
    public QuotaType Entity2Type { get; set; }
    public int Entity2Id { get; set; }
    public PeriodType PeriodType { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Total { get; set; }
    public int Used { get; set; }
    public int Reserved { get; set; }
}