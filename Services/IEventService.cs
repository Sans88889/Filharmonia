using Filharmonia.Services;

public interface IEventService
{
    IEnumerable<EventReport> GetEventReport();
}
