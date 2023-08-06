using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.CQRS.Commands;
using Project.CQRS.Handlers;
using Project.CQRS.Queries;
using Project.CQRS.Results;
using Project.Dal;
using Project.Models;

namespace Project.Controllers
{
    [Produces("application/json")]
    [Route("api/Events")]
    public class EventsController : Controller
    {
        private readonly CalendarContext _context;
        private readonly GetEventQueryHandler _getEventQueryHandler;
        private readonly CreateEventCommandHandler _createEventCommandHandler;
        private readonly RemoveEventCommandHandler _removeEventCommandHandler;

        public EventsController(CalendarContext context,
               GetEventQueryHandler getEventQueryHandler,
               CreateEventCommandHandler createEventCommandHandler,
               RemoveEventCommandHandler removeEventCommandHandler)
        {
            _context = context;
            _getEventQueryHandler = getEventQueryHandler;
            _createEventCommandHandler = createEventCommandHandler;
            _removeEventCommandHandler = removeEventCommandHandler;
        }


        // GET: api/Events
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
           
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

                var query = new GetEventQueryResult { Start = start, End = end };
                var events = await _getEventQueryHandler.Handle(query);

                return Ok(events);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            @event.Start = DateTime.SpecifyKind(@event.Start, DateTimeKind.Utc);
            @event.End = DateTime.SpecifyKind(@event.End, DateTimeKind.Utc);

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent([FromRoute] int id, [FromBody] Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @event.Id)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

    
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveEvent([FromRoute] int id, [FromBody] EventMoveParams param)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.Start = param.Start;
            @event.End = param.End;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

      
        [HttpPut("{id}/color")]
        public async Task<IActionResult> SetEventColor([FromRoute] int id, [FromBody] EventColorParams param)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.Color = param.Color;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostEvent([FromBody] CreateEventCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                await _createEventCommandHandler.Handle(command);
                return CreatedAtAction("GetEvent", new { id = command.Id }, command);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new RemoveEventCommand { Id = id };
                await _removeEventCommandHandler.Handle(command);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
            }
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }


    public class EventMoveParams
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class EventColorParams
    {
        public string Color { get; set; }
    }

}