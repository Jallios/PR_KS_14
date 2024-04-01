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
    public class TasksController : ControllerBase
    {
        private readonly TaskTrackerContext _context;

        public TasksController(TaskTrackerContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet("userTasks")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetUserTasks()
        {
            return await _context.Tasks
                .Include(x => x.Project)
                .Include(x => x.Priority)
                .Include(x => x.Status)
                .Include(x => x.Executor)
                .Include(x => x.Creator).
                Where(x => x.ExecutorId == Decoder.getUserId(Request.Headers.Authorization) && x.CreatorId == Decoder.getUserId(Request.Headers.Authorization)).ToListAsync();
        }

        [HttpGet("projectTasks")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetProjectTasks([FromBody] long? projectId)
        {
            return await _context.Tasks.Include(x => x.Priority)
                .Include(x => x.Status)
                .Include(x => x.Executor)
                .Include(x => x.Creator).
                Where(x => x.ProjectId == projectId ).ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("id")]
        public async Task<ActionResult<Models.Task>> GetTask([FromBody]long? id)
        {
            var task = await _context.Tasks.Include(x => x.Priority)
                .Include(x => x.Status)
                .Include(x => x.Executor)
                .Include(x => x.Creator).FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(long? id, Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Models.Task>> PostTask(Models.Task task)
        {
            task.CreatorId = Decoder.getUserId(Request.Headers.Authorization);
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long? id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(long? id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
