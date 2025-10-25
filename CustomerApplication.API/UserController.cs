using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.DTOs.User.Queries;
using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.Application.Common.Models; // IUserService

namespace YourProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // POST: api/users
        // Create if Id is null/empty; Update if Id provided
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] CreateOrUpdateUserCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(command.Username))
                return BadRequest("Username is required.");
            

            var id = await _userService.CreateAsync(command);
            return Ok(new { id });
        }

        // GET: api/users/{id}?includeRole=true
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> GetById(Guid id, [FromQuery] bool includeRole = false)
        {
            var user = await _userService.GetByIdAsync(id, includeRole);
            if (user is null)
                return NotFound();

            var dto = _mapper.Map<User>(user);
            return Ok(dto);
        }

        // GET: api/users/by-username/{username}
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<User>> GetByUsername(string username, [FromQuery] bool includeRole = false)
        {
            var user = await _userService.GetByUsernameAsync(username);
            if (user is null)
                return NotFound();

            // If caller asked for role, fetch with include (optional)
            if (includeRole && user.Role == null)
            {
                var reloaded = await _userService.GetByIdAsync(user.Id, includeRole: true);
                if (reloaded != null) user = reloaded;
            }

            var dto = _mapper.Map<User>(user);
            return Ok(dto);
        }

        // GET: api/users?includeRoles=true&skip=0&take=50
        [HttpGet]
        public async Task<ActionResult<PagedResult<User>>> GetList(
            [FromQuery] bool includeRoles = false,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            if (take <= 0) take = 50;
            if (skip < 0) skip = 0;

            var users = (await _userService.GetListAsync(includeRoles)).ToList();

            var pageItems = users
                .Skip(skip)
                .Take(take)
                .Select(u => _mapper.Map<User>(u))
                .ToList();

            var result = new PagedResult<User>
            {
                Items = pageItems,
                TotalCount = users.Count
            };

            return Ok(result);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
