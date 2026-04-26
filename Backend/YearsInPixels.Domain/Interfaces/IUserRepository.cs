using YearsInPixels.Domain.Entities;

namespace YearsInPixels.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AnyUserAsync(string username);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
