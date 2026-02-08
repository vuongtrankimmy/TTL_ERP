using Microsoft.AspNetCore.Mvc;
using TTL.HR.Shared.Entities.HumanResource;
using TTL.HR.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IRepository<Employee> _repository;

        public EmployeeController(IRepository<Employee> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(string id)
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Post([FromBody] Employee employee)
        {
            await _repository.CreateAsync(employee);
            return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Employee employee)
        {
            if (id != employee.Id) return BadRequest();
            await _repository.UpdateAsync(id, employee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
