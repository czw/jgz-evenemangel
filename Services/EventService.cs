using jgz_evenemangel.Controllers;
using jgz_evenemangel.Extensions;
using jgz_evenemangel.Models.Documents;
using jgz_evenemangel.Models.Dtos;
using jgz_evenemangel.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace jgz_evenemangel.Services;

public class EventService(IMongoDatabase database, IOptions<DatabaseOptions> databaseSettings)
{
    private readonly IMongoCollection<AttendeeDocument> _attendeesCollection =
        database.GetCollection<AttendeeDocument>(databaseSettings.Value.AttendeesCollectionName);

    private readonly IMongoCollection<EventDocument> _eventsCollection =
        database.GetCollection<EventDocument>(databaseSettings.Value.EventsCollectionName);

    public async Task<List<EventDocument>> GetAsync()
    {
        return await _eventsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<EventDocument?> GetAsync(string id)
    {
        return await _eventsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<long> GetAttendeeCountAsync(string eventId)
    {
        return await _attendeesCollection.CountDocumentsAsync(a => a.EventId == eventId);
    }

    public async Task RegisterAttendeesAsync(string eventId, List<Registration> registrations)
    {
        if (registrations.Count == 0)
            return;

        var firstAttendeeName = registrations[0].Name;
        var attendees = registrations.Select((registration, index) => new AttendeeDocument
        {
            EventId = eventId, Name = registration.Name, Diet = registration.Diet, Allergies = registration.Allergies,
            PartnerName = index == 0 ? null : firstAttendeeName
        }).ToList();
        await _attendeesCollection.InsertManyAsync(attendees);
    }

    public async Task<List<AttendeeDto>> GetAttendeesForEventAsync(string eventId)
    {
        var attendeeDocuments = await _attendeesCollection
            .Find(a => a.EventId == eventId)
            .ToListAsync();

        return attendeeDocuments.Select(doc => doc.ToDto()).ToList();
    }
}