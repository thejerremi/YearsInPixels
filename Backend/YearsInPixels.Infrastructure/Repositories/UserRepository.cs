using Microsoft.EntityFrameworkCore;
using YearsInPixels.Domain.Entities;
using YearsInPixels.Domain.Interfaces;

namespace YearsInPixels.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AnyUserAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
