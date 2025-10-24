using CustomerApplication.Data;
using Microsoft.EntityFrameworkCore;
using static CustomerApplication.CustomerApplication.Domain.Enums.LookupEnums;

public interface ILookupService
{
    Task<PagedResult<LookupDto>> SearchAsync(LookupSearchRequest request, CancellationToken ct = default);
}

public sealed class LookupService : ILookupService
{
    private readonly AppDbContext _ctx;
    public LookupService(AppDbContext ctx) => _ctx = ctx;

    public async Task<PagedResult<LookupDto>> SearchAsync(LookupSearchRequest r, CancellationToken ct = default)
    {
        if (!Enum.IsDefined(typeof(CategoryCode), r.CategoryCode))
            throw new ArgumentException("Invalid CategoryCode.", nameof(r.CategoryCode));

        var take = Math.Clamp(r.Take, 1, 200);
        var skip = Math.Max(0, r.Skip);

        // Filter lookups by CategoryCode
        var query = _ctx.Lookups
                        .AsNoTracking()
                        .Where(x => x.CategoryCode == r.CategoryCode);

        if (r.ParentId.HasValue)
            query = query.Where(x => x.ParentId == r.ParentId.Value);

        if (r.IsActive.HasValue)
            query = query.Where(x => x.IsActive == r.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(r.Q))
        {
            var term = $"%{r.Q.Trim()}%";
            query = query.Where(x =>
                (x.Name != null && EF.Functions.Like(x.Name, term)) ||
                (x.Code != null && EF.Functions.Like(x.Code, term)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .Select(x => new LookupDto
            {
                Id = x.Id,
                CategoryCode = (CategoryCode)x.CategoryCode, // cast int to enum
                ParentId = x.ParentId,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(ct);

        return new PagedResult<LookupDto>
        {
            TotalCount = total,
            Items = items
        };
    }
}
