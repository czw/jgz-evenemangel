using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace jgz_evenemangel.Models.Documents;

public class EventDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("name")] public required string Name { get; set; }

    [BsonElement("date")] public required DateTime Date { get; set; }

    [BsonElement("registrationDate")] public required DateTime RegistrationDate { get; set; }

    [BsonElement("maxAttendees")] public required int MaxAttendees { get; set; }

    [BsonElement("hardAttendeeLimit")] public bool HardAttendeeLimit { get; set; }
}