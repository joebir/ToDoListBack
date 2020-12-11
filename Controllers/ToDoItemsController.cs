using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListBack.Models;

namespace ToDoListBack.Controllers
{
    [Route("")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly ToDoItemContext _context;

        public ToDoItemsController(ToDoItemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoItemDTO>>> GetToDoItems()
        {
            return await _context.ToDoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItemDTO>> GetToDoItem(long id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(toDoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDoItem(long id, ToDoItemDTO ToDoItemDTO)
        {
            if (id != ToDoItemDTO.Id)
            {
                return BadRequest();
            }

            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            toDoItem.Name = ToDoItemDTO.Name;
            toDoItem.IsComplete = ToDoItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ToDoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ToDoItemDTO>> CreateToDoItem(ToDoItemDTO ToDoItemDTO)
        {
            var toDoItem = new ToDoItem
            {
                IsComplete = ToDoItemDTO.IsComplete,
                Name = ToDoItemDTO.Name,
                Created = DateTime.UtcNow,
            };

            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetToDoItem),
                new { id = toDoItem.Id },
                ItemToDTO(toDoItem));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(long id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoItemExists(long id) =>
             _context.ToDoItems.Any(e => e.Id == id);

        private static ToDoItemDTO ItemToDTO(ToDoItem toDoItem) =>
            new ToDoItemDTO
            {
                Id = toDoItem.Id,
                Name = toDoItem.Name,
                IsComplete = toDoItem.IsComplete,
                Created = toDoItem.Created
            };
    }
}
