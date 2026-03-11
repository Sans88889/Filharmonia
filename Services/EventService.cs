using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Filharmonia.Services
{
    public class EventService : IEventService
    {
        public IEnumerable<EventReport> GetEventReport()
        {
            return new List<EventReport>
    {
        new EventReport { EventName = "Koncert Beethovena", TicketsSold = 50 },
        new EventReport { EventName = "Jazz Night", TicketsSold = 30 },
        new EventReport { EventName = "Chopin Gala", TicketsSold = 20 }
    };
        }


    }
}
