using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StealthChallenge.Abstractions.Domain.Services;
using StealthChallenge.Abstractions.Infrastructure.Services;

namespace StealthChallenge.Infrastructure
{
    public class RankingRangeMatchmaker : IMakeMatches
    {
        readonly IUserService _userService;
        private readonly int _range;

        public RankingRangeMatchmaker(IUserService userService, int range)
        {
            _userService = userService;
            _range = Math.Abs(range);
        }

        public async Task<IEnumerable<string>> Match(string userName)
        {
            var user = await _userService.FindByIdAsync(userName);
            if (user is null) return Enumerable.Empty<string>();

            var userRankProjections = await _userService.FindFriendsAsync(userName);
            return userRankProjections
                .Where(x => InRange(x.Ranking, user.Ranking - _range, user.Ranking + _range))
                .Select(x => x.Name);
        }

        internal bool InRange(int v, int v1, int v2)
        {
            if (v1 > v2) throw new ArgumentException($"Invalid range [{v1},{v2}]");
            if (v < v1) return false;
            if (v > v2) return false;
            return true;
        }
    }
}
