using Project.CQRS.Commands;
using Project.Dal;
using Project.Models;

namespace Project.CQRS.Handlers
{
    public class CreateEventCommandHandler
    {
        private readonly CalendarContext _context;

        public CreateEventCommandHandler(CalendarContext context)
        {
            _context = context;
        }
        public async Task Handle(CreateEventCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var @event = new Event
            {
                
                Start = DateTime.SpecifyKind(command.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(command.End, DateTimeKind.Utc),
                Text = command.Text,
                Color = command.Color
            };

            _context.Events.Add(@event);
            await _context.SaveChangesAsync();
        }
    }
}