using muatamer_camunda_poc.Enum;

namespace muatamer_camunda_poc.Models;

public class UmrahOperator
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? MobileNumber { get; set; }
    public State State { get; set; }
    public virtual ICollection<ExternalAgentUmrahOperator> ExternalAgentUmrahOperators { get; set; }
    public virtual ICollection<UmrahGroup> UmrahGroups { get; set; }
}
