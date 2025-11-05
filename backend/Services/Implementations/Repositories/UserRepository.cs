using CloudCore.Data.Context;
using CloudCore.Domain.Entities;
using CloudCore.Services.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CloudCore.Services.Implementations.Repositories
{
    public class UserRepository(CloudCoreDbContext context, ILogger<UserRepository> logger) : IUserRepository
    {
        public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Users
                .FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<User?> GetUserByNameAsync(string username, CancellationToken cancellationToken)
        {
            return await context.Users
                  .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> CheckUserExistsAsync(string username, string email, CancellationToken cancellationToken)
        {
            return await context.Users
                .AnyAsync(u => u.Username == username || u.Email == email, cancellationToken);
        }

        public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
        {

            return await context.Users
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {

            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
