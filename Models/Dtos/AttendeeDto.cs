namespace jgz_evenemangel.Models.Dtos;

public class AttendeeDto
{
    public required string Id { get; set; }
    public required string EventId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public required string Name { get; set; }
    public required string Diet { get; set; }
    public required string Allergies { get; set; }
    public string? PartnerName { get; set; }
}