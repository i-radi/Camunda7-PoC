namespace muatamer_camunda_poc.Models;

public class MuatamerInformation
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int NationalityId { get; set; }
    public Nationality Nationality { get; set; }
    public string? PassportType { get; set; }
    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public DateTime PassportExpiryDate { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public string? Gender { get; set; }
    public int GroupId { get; set; }
    public virtual UmrahGroup Group { get; set; }
}
