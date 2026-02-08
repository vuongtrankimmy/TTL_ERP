using Microsoft.AspNetCore.Mvc;
using TTL.HR.Shared.Entities.System;
using TTL.HR.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRepository<Role> _repository;

        public RoleController(IRepository<Role> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> Get()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> Get(string id)
        {
            var role = await _repository.GetByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> Post([FromBody] Role role)
        {
            await _repository.CreateAsync(role);
            return CreatedAtAction(nameof(Get), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Role role)
        {
            if (id != role.Id) return BadRequest();
            await _repository.UpdateAsync(id, role);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IRepository<Permission> _repository;

        public PermissionController(IRepository<Permission> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> Get()
        {
            return Ok(await _repository.GetAllAsync());
        }
    }
}
