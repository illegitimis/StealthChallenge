using StealthChallenge.Abstractions.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealthChallenge.Abstractions.Domain.Services
{
    public interface IUserService
    {
        Task<User> FindByIdAsync(string id);
        Task<IEnumerable<UserRankProjection>> FindFriendsAsync(string userName);
        Task UpdateRankingAsync(string user, int score);
    }
}
