using jgz_evenemangel.Models.Documents;
using jgz_evenemangel.Models.Dtos;

namespace jgz_evenemangel.Extensions;

public static class MappingExtensions
{
    public static AttendeeDto ToDto(this AttendeeDocument document)
    {
        return new AttendeeDto
        {
            Id = document.Id,
            EventId = document.EventId,
            RegistrationDate = document.RegistrationDate,
            Name = document.Name,
            Diet = document.Diet,
            Allergies = document.Allergies,
            PartnerName = document.PartnerName
        };
    }

    public static EventDto ToDto(this EventDocument document, long registeredCount)
    {
        return new EventDto
        {
            Id = document.Id,
            Name = document.Name,
            Date = document.Date,
            RegistrationDate = document.RegistrationDate,
            MaxAttendees = document.MaxAttendees,
            HardAttendeeLimit = document.HardAttendeeLimit,
            RegisteredCount = registeredCount
        };
    }
}