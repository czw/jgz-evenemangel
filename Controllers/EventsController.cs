using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net.Mime;
using JetBrains.Annotations;
using jgz_evenemangel.Extensions;
using jgz_evenemangel.Models.Dtos;
using jgz_evenemangel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace jgz_evenemangel.Controllers;

public static class CacheKeys
{
    public static string AllEvents => "_AllEvents";

    public static string EventById(string id)
    {
        return $"_Event_{id}";
    }
}

[UsedImplicitly]
public class Registration
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public required string Name { get; init; }
    public required string Diet { get; init; }

    public required string Allergies { get; init; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
}

[ApiController]
[Description("Controller for managing events and event registrations")]
[Route("api/v1/events")]
[Tags("Events")]
[Produces(MediaTypeNames.Application.Json)]
public class EventsController(
    ApiKeyService apiKeyService,
    EventService eventService,
    ILogger<EventsController> logger,
    IMemoryCache memoryCache)
    : ControllerBase
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> RegistrationSemaphores = new();

    [HttpGet]
    [EndpointSummary("Get all events")]
    [ProducesResponseType<List<EventDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<EventDto>>> GetEvents()
    {
        if (memoryCache.TryGetValue(CacheKeys.AllEvents, out List<EventDto>? cachedEvents))
            if (cachedEvents != null)
                return Ok(cachedEvents);

        var events = await eventService.GetAsync();
        memoryCache.Set(CacheKeys.AllEvents, events);
        return Ok(events);
    }

    [HttpGet("{id:length(24)}", Name = "GetEvent")]
    [EndpointSummary("Get a specific event by ID")]
    [ProducesResponseType<EventDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventDto>> GetEvent(
        [Description("The unique identifier of the event")]
        string id)
    {
        var cacheKey = CacheKeys.EventById(id);
        if (memoryCache.TryGetValue(cacheKey, out EventDto? cachedEventDto))
            if (cachedEventDto != null)
                return Ok(cachedEventDto);

        var eventDocument = await eventService.GetAsync(id);
        if (eventDocument == null)
        {
            logger.LogWarning("Event with id {id} not found.", id);
            return NotFound($"Event with id {id} not found.");
        }

        var registeredCount = await eventService.GetAttendeeCountAsync(id);
        var eventDto = eventDocument.ToDto(registeredCount);
        memoryCache.Set(cacheKey, eventDto);
        return Ok(eventDto);
    }

    [HttpPost("{eventId:length(24)}/register")]
    [EndpointSummary("Register for an event")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<int[]>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterForEvent(string eventId, [FromBody] List<Registration> registrations)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var eventDocument = await eventService.GetAsync(eventId);
        if (eventDocument == null)
            return NotFound($"Event with id {eventId} not found.");

        var semaphore = RegistrationSemaphores.GetOrAdd(eventId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            var registeredCount = await eventService.GetAttendeeCountAsync(eventDocument.Id);
            if (eventDocument.HardAttendeeLimit && registeredCount >= eventDocument.MaxAttendees)
                return Conflict("Event is already full.");

            await eventService.RegisterAttendeesAsync(eventId, registrations);
            memoryCache.Remove(CacheKeys.EventById(eventId));
            memoryCache.Remove(CacheKeys.AllEvents);
            var count = (int)await eventService.GetAttendeeCountAsync(eventId);
            var registeredIndices = Enumerable.Range(count - registrations.Count + 1, count).ToArray();
            return CreatedAtAction(nameof(RegisterForEvent), registeredIndices);
        }
        finally
        {
            semaphore.Release();
        }
    }

    // GET: api/events/{eventId}/registrations?apiKey={apiKey}
    [HttpGet("{eventId:length(24)}/registrations")]
    [ProducesResponseType<List<AttendeeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AttendeeDto>>> GetEventRegistrations(string eventId, [FromQuery] string? apiKey)
    {
        if (!apiKeyService.IsValidApiKey(apiKey))
        {
            logger.LogWarning("Invalid or missing API key attempt for event {EventId}", eventId);
            return Unauthorized("Invalid or missing API key.");
        }

        var eventItem = await eventService.GetAsync(eventId);
        if (eventItem == null)
            return NotFound($"Event with id {eventId} not found.");

        var attendees = await eventService.GetAttendeesForEventAsync(eventId);
        logger.LogInformation("Retrieved {Count} registrations for event {EventId} using API key.", attendees.Count,
            eventId);
        return Ok(attendees);
    }
}