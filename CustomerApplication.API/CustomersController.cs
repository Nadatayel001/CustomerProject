
using CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
     [FromQuery] int skip = 0,
     [FromQuery] int take = 20,
     [FromQuery] string? search = null)
        {
            var customers = await _customerService.GetPagedAsync(skip, take, search);
            return Ok(customers);
        }

        // 🔹 GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                    return NotFound(new { Message = "Customer not found" });

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // 🔹 POST: api/customer
        // Handles both Create & Update
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] Command dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = await _customerService.CreateOrUpdateAsync(dto);
                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // 🔹 DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _customerService.DeleteAsync(id);
                if (!success)
                    return NotFound(new { Message = "Customer not found or already deleted" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
