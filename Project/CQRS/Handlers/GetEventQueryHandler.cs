using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Models; // Assuming you have a model named Event for your database entity.
using Project.Dal;
using Project.CQRS.Results;

namespace Project.CQRS.Queries
{

    public class GetEventQueryHandler
    {
        private readonly CalendarContext _context;

        public GetEventQueryHandler(CalendarContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> Handle(GetEventQueryResult query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            query.Start = DateTime.SpecifyKind(query.Start, DateTimeKind.Utc);
            query.End = DateTime.SpecifyKind(query.End, DateTimeKind.Utc);

            var events = await _context.Events
                .Where(e => !(e.End <= query.Start || e.Start >= query.End))
                .ToListAsync();

            return events;
        }
    }
}
