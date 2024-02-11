using muatamer_camunda_poc.Enum;

namespace muatamer_camunda_poc.Models;

public class ExternalAgent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public State State { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public virtual ICollection<UmrahOperator> UmrahOperators { get; set; }

}
