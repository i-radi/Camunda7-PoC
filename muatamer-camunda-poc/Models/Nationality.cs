using muatamer_camunda_poc.Enum;

namespace muatamer_camunda_poc.Models;

public class Nationality
{
    public int Id { get; set; }
    public string Name { get; set; }
    public State State { get; set; }
}
