using Microsoft.AspNetCore.Mvc;

namespace CustomerApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupService _service;

        public LookupsController(ILookupService service)
        {
            _service = service;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<LookupDto>>> Search(
            [FromQuery] int categoryCode,
            [FromQuery] Guid? parentId,
            [FromQuery] string? q,
            [FromQuery] bool? isActive,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            var request = new LookupSearchRequest
            {
                CategoryCode = categoryCode,
                ParentId = parentId,
                Q = q,
                IsActive = isActive,
                Skip = skip,
                Take = Math.Clamp(take, 1, 200) 
            };

            var result = await _service.SearchAsync(request, ct);
            return Ok(result);
        }

        
        [HttpGet("{categoryCode}")]
        public async Task<ActionResult<PagedResult<LookupDto>>> ByCategory(
            [FromRoute] int categoryCode,
            [FromQuery] Guid? parentId,
            [FromQuery] string? q,
            [FromQuery] Guid? id,
            [FromQuery] bool? isActive,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            var result = await _service.SearchAsync(new LookupSearchRequest
            {
                Id=id,
                CategoryCode = categoryCode,
                ParentId = parentId,
                Q = q,
                IsActive = isActive,
                Skip = skip,
                Take = Math.Clamp(take, 1, 200)
            }, ct);

            return Ok(result);
        }
    }
}
