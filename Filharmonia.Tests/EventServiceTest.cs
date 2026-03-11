using Filharmonia.Services;
using Moq;
using Filharmonia.Models;

public class EventServiceTests
{
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _eventService = new EventService();
    }

    [Fact]
    public void GetEventReport_ShouldReturnCorrectNumberOfEvents()
    {
        var result = _eventService.GetEventReport();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetEventReport_ShouldContainCorrectEventNames()
    {
        var result = _eventService.GetEventReport();
        var eventNames = result.Select(e => e.EventName).ToList();

        Assert.Contains("Koncert Beethovena", eventNames);
        Assert.Contains("Jazz Night", eventNames);
        Assert.Contains("Chopin Gala", eventNames);
    }

    [Fact]
    public void GetEventReport_ShouldContainCorrectTicketsSold()
    {
        var result = _eventService.GetEventReport();
        var ticketsSold = result.Select(e => e.TicketsSold).ToList();

        Assert.Contains(50, ticketsSold);
        Assert.Contains(30, ticketsSold);
        Assert.Contains(20, ticketsSold);
    }

    [Fact]
    public void GetEventReport_ShouldNotReturnNull()
    {
        var result = _eventService.GetEventReport();

        Assert.NotNull(result);
    }

    [Fact]
    public void GetEventReport_ShouldReturnEventsInCorrectOrder()
    {
        var result = _eventService.GetEventReport().ToList();

        Assert.Equal("Koncert Beethovena", result[0].EventName);
        Assert.Equal("Jazz Night", result[1].EventName);
        Assert.Equal("Chopin Gala", result[2].EventName);
    }

    [Fact]
    public void GetEventReport_ShouldReturnUniqueEvents()
    {
        var result = _eventService.GetEventReport();

        var eventNames = result.Select(e => e.EventName).Distinct();
        Assert.Equal(3, eventNames.Count());
    }

    [Fact]
    public void GetEventReport_ShouldNotReturnNegativeTicketCounts()
    {
        var result = _eventService.GetEventReport();

        Assert.All(result, e => Assert.True(e.TicketsSold >= 0));
    }

    [Fact]
    public void GetEventReport_ShouldHandleNullValuesInData()
    {
        // Arrange
        var eventService = new EventService();
        var events = eventService.GetEventReport().ToList();

        // Act
        var result = events.Any(e => e.EventName == null || e.TicketsSold < 0);

        // Assert
        Assert.False(result, "EventReport contains null values or invalid ticket counts.");
    }

    [Fact]
    public void GetEventReport_ShouldReturnNonEmptyList()
    {
        // Arrange
        var eventService = new EventService();

        // Act
        var result = eventService.GetEventReport();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
