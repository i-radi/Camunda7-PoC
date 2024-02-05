namespace muatamer_camunda_poc.Models;

public class MuatamerInformation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NationalityId { get; set; }
    public string PassportType { get; set; }
    public string PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public DateTime PassportExpiryDate { get; set; }
    public string CountryName { get; set; }
    public string Gender { get; set; }
    public int GroupId { get; set; }
    public virtual UmrahGroup Group { get; set; }
}
