using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.Data;
using Microsoft.EntityFrameworkCore;

public interface ILookupRepository
{
    Task<PagedResult<Lookup>> SearchAsync(
        int categoryCode,
        Guid? parentId,
        string? q,
        bool? isActive,
        int skip,
        int take,
        CancellationToken ct = default);
}

public sealed class LookupRepository : ILookupRepository
{
    private readonly AppDbContext _ctx;
    public LookupRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<PagedResult<Lookup>> SearchAsync(
        int categoryCode, Guid? parentId, string? q, bool? isActive, int skip, int take, CancellationToken ct = default)
    {
        var query = _ctx.Lookups.AsQueryable();

        query = query.Where(x => x.CategoryCode == categoryCode);

        if (parentId.HasValue)
            query = query.Where(x => x.ParentId == parentId.Value);

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(x =>
                (x.Name != null && x.Name.Contains(term)) ||
                (x.Code != null && x.Code.Contains(term)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Name)         
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return new PagedResult<Lookup> { TotalCount = total, Items = items };
    }
}
