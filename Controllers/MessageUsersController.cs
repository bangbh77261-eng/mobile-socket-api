using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mobile_api_test.Data;
using mobile_api_test.Models;

namespace mobile_api_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessageUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MessageUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageUser>>> GetMessageUser()
        {
            return await _context.MessageUser.ToListAsync();
        }

        // GET: api/MessageUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageUser>> GetMessageUser(int id)
        {
            var messageUser = await _context.MessageUser.FindAsync(id);

            if (messageUser == null)
            {
                return NotFound();
            }

            return messageUser;
        }

        // PUT: api/MessageUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessageUser(int id, MessageUser messageUser)
        {
            if (id != messageUser.MessageId)
            {
                return BadRequest();
            }

            _context.Entry(messageUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageUserExists(id))
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

        // POST: api/MessageUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MessageUser>> PostMessageUser(MessageUser messageUser)
        {
            _context.MessageUser.Add(messageUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessageUser", new { id = messageUser.MessageId }, messageUser);
        }

        // DELETE: api/MessageUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessageUser(int id)
        {
            var messageUser = await _context.MessageUser.FindAsync(id);
            if (messageUser == null)
            {
                return NotFound();
            }

            _context.MessageUser.Remove(messageUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageUserExists(int id)
        {
            return _context.MessageUser.Any(e => e.MessageId == id);
        }
    }
}
