using System.ComponentModel;

namespace jgz_evenemangel.Models.Dtos;

/// <summary>
///     Represents an event with all its details including registration information
/// </summary>
public record EventDto
{
    /// <summary>
    ///     Unique identifier for the event
    /// </summary>
    /// <example>507f1f77bcf86cd799439011</example>
    [Description("The unique identifier for the event")]
    public required string Id { get; set; }

    /// <summary>
    ///     Name of the event
    /// </summary>
    /// <example>Annual Company Meeting</example>
    [Description("The name of the event")]
    public required string Name { get; set; }

    /// <summary>
    ///     Date and time when the event takes place
    /// </summary>
    /// <example>2024-12-15T14:30:00Z</example>
    [Description("The date and time when the event takes place")]
    public required DateTime Date { get; set; }

    /// <summary>
    ///     Date and time when registration opens/opened
    /// </summary>
    /// <example>2024-11-01T09:00:00Z</example>
    [Description("The date and time when registration opens")]
    public required DateTime RegistrationDate { get; set; }

    /// <summary>
    ///     Maximum number of attendees allowed
    /// </summary>
    /// <example>100</example>
    [Description("Maximum number of attendees allowed for the event")]
    public required int MaxAttendees { get; set; }

    /// <summary>
    ///     Whether the attendee limit is strictly enforced
    /// </summary>
    /// <example>true</example>
    [Description("Indicates if the attendee limit is strictly enforced")]
    public required bool HardAttendeeLimit { get; set; }

    /// <summary>
    ///     Current number of registered attendees
    /// </summary>
    /// <example>75</example>
    [Description("Current number of people registered for the event")]
    public required long RegisteredCount { get; set; }
}