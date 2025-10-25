using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Domain.Repositories;
using CustomerApplication.Data;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByIdWithRoleAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.Include(x=>x.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ToListAsync();
    }



    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        User.Delete(user);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
    }

}
