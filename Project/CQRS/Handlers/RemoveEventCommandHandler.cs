using Microsoft.EntityFrameworkCore;
using Project.CQRS.Commands;
using Project.Dal;

namespace Project.CQRS.Handlers
{
    public class RemoveEventCommandHandler
    {
        private readonly CalendarContext _context;

        public RemoveEventCommandHandler(CalendarContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveEventCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == command.Id);
            if (@event == null)
            {
                throw new ArgumentException($"Event with ID {command.Id} not found.");
            }
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }
    }
}
