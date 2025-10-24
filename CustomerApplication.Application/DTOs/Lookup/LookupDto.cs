public sealed class LookupDto
{
    public Guid Id { get; set; }

    public int CategoryCode { get; set; }

    public Guid? ParentId { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = default!;
}

public sealed class LookupSearchRequest
{
    public int CategoryCode { get; set; }
    public Guid? Id { get; set; }
    public Guid? ParentId { get; set; }
    public string? Q { get; set; }
    public bool? IsActive { get; set; }

    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}

public sealed class PagedResult<T>
{
    public int TotalCount { get; set; }
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
}
