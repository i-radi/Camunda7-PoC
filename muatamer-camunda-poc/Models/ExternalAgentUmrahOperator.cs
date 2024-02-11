namespace muatamer_camunda_poc.Models;

public class ExternalAgentUmrahOperator
{
    public int Id { get; set; }
    public int ExternalAgentId { get; set; }
    public ExternalAgent ExternalAgent { get; set; }
    public int UmrahOperatorId { get; set; }
    public UmrahOperator UmrahOperator { get; set; }
}
