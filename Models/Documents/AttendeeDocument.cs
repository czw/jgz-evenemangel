using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace jgz_evenemangel.Models.Documents;

public class AttendeeDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("eventId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string EventId { get; init; }

    [BsonElement("registrationDate")] public DateTime RegistrationDate { get; init; } = DateTime.UtcNow;

    [BsonElement("name")] public required string Name { get; init; }
    [BsonElement("diet")] public required string Diet { get; init; }
    [BsonElement("allergies")] public required string Allergies { get; init; }
    [BsonElement("partnerName")] public string? PartnerName { get; init; }
}