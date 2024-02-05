namespace muatamer_camunda_poc.Models;

public class UmrahGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public string FromCountry { get; set; }
    public bool IsActive { get; set; }
    public bool HasVoucher { get; set; }
    public bool VisaIssued { get; set; }
    public string Country { get; set; }
    public virtual ICollection<MuatamerInformation> MuatamerInformations { get; set; }
}
