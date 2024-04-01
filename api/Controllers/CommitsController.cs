using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using api.Options;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommitsController : ControllerBase
    {
        private readonly TaskTrackerContext _context;

        public CommitsController(TaskTrackerContext context)
        {
            _context = context;
        }

        // GET: api/Commits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commit>>> GetCommits([FromBody]long? taskId)
        {
            return await _context.Commits.Include(x => x.Task).Include(x => x.User).Where(x => x.TaskId == taskId).ToListAsync();
        }

        // GET: api/Commits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commit>> GetCommit(long? id)
        {
            var commit = await _context.Commits.FindAsync(id);

            if (commit == null)
            {
                return NotFound();
            }

            return commit;
        }

        // PUT: api/Commits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommit(long? id, Commit commit)
        {
            if (id != commit.Id)
            {
                return BadRequest();
            }

            _context.Entry(commit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommitExists(id))
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

        // POST: api/Commits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Commit>> PostCommit(Commit commit)
        {
            commit.UserId = Decoder.getUserId(Request.Headers.Authorization);
            _context.Commits.Add(commit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommit", new { id = commit.Id }, commit);
        }

        // DELETE: api/Commits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommit(long? id)
        {
            var commit = await _context.Commits.FindAsync(id);
            if (commit == null)
            {
                return NotFound();
            }

            _context.Commits.Remove(commit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommitExists(long? id)
        {
            return _context.Commits.Any(e => e.Id == id);
        }
    }
}
