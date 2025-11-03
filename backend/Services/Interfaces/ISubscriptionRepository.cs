using CloudCore.Common.Models;

namespace CloudCore.Services.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<TeamspaceLimits> GetTeamspaceLimitsAsync(int userId);
        Task<bool> CanCreateTeamspaceAsync(int userId);
    }
}