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
    public class ProjectUserRolesController : ControllerBase
    {
        private readonly TaskTrackerContext _context;

        public ProjectUserRolesController(TaskTrackerContext context)
        {
            _context = context;
        }

        // GET: api/ProjectUserRoles
        [HttpGet("userProjects")]
        public async Task<ActionResult<IEnumerable<ProjectUserRole>>> GetProjectsUserRoles()
        {
            return await _context.ProjectUserRoles.Include(x => x.Project).Where(x => x.UserId == Decoder.getUserId(Request.Headers.Authorization)).ToListAsync();
        }

        [HttpGet("projectUsers")]
        public async Task<ActionResult<IEnumerable<ProjectUserRole>>> GetUsersProjectRoles([FromBody] int projectId)
        {
            return await _context.ProjectUserRoles.Include(x => x.User).Include(x => x.Role).Where(x => x.ProjectId == projectId).ToListAsync();
        }

        // GET: api/ProjectUserRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectUserRole>> GetProjectUserRole(long? id)
        {
            var projectUserRole = await _context.ProjectUserRoles.FindAsync(id);

            if (projectUserRole == null)
            {
                return NotFound();
            }

            return projectUserRole;
        }

        // PUT: api/ProjectUserRoles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectUserRole(long? id, ProjectUserRole projectUserRole)
        {
            if (id != projectUserRole.Id)
            {
                return BadRequest();
            }

            _context.Entry(projectUserRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectUserRoleExists(id))
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

        // POST: api/ProjectUserRoles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectUserRole>> PostProjectUserRole(ProjectUserRole projectUserRole)
        {
            _context.Projects.Add(projectUserRole.Project);
            await _context.SaveChangesAsync();

            projectUserRole.ProjectId = projectUserRole.Project.Id;
            projectUserRole.UserId = Decoder.getUserId(Request.Headers.Authorization);
            projectUserRole.RoleId = 1;

            _context.ProjectUserRoles.Add(projectUserRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectUserRole", new { id = projectUserRole.Id }, projectUserRole);
        }

        [HttpPost("addUsers")]
        public async Task<ActionResult<ProjectUserRole>> PostProjectUsersRole(List<ProjectUserRole> projectUsersRole)
        {
           
            _context.ProjectUserRoles.AddRange(projectUsersRole);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/ProjectUserRoles/5
        [HttpDelete]
        public async Task<IActionResult> DeleteProjectUserRole([FromBody]long? id)
        {
            var projectUserRole = await _context.ProjectUserRoles.FindAsync(id);
            if (projectUserRole == null)
            {
                return NotFound();
            }

            _context.ProjectUserRoles.Remove(projectUserRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectUserRoleExists(long? id)
        {
            return _context.ProjectUserRoles.Any(e => e.Id == id);
        }
    }
}
