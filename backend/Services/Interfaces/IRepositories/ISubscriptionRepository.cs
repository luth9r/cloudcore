using CloudCore.Common.Models;

namespace CloudCore.Services.Interfaces.IRepositories
{
    public interface ISubscriptionRepository
    {
        Task<TeamspaceLimits> GetTeamspaceLimitsAsync(int userId);
        Task<bool> CanCreateTeamspaceAsync(int userId);
    }
}