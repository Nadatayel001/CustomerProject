using CustomerApplication.CustomerApplication.Application.DTOs.Role.Commands.CreateOrUpdate;
using Microsoft.AspNetCore.Mvc;

namespace YourProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] CreateOrUpdateRoleCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _roleService.CreateAsync(command);
            return Ok(new { id }); // Could also return role details if needed
        }

        // GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        // GET: api/roles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetListAsync();
            return Ok(roles);
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
