using StealthChallenge.Abstractions.Domain.Models;
using StealthChallenge.Abstractions.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StealthChallenge.Domain.Services
{
    public class InMemoryStubUserService : IUserService
    {
        public Task<User> FindByIdAsync(string id) =>
            Task.FromResult(Users.SingleOrDefault(x => x.Name == id));

        public Task<IEnumerable<UserRankProjection>> FindFriendsAsync(string userName) =>
            Task.FromResult(FindFriends(userName));

        public Task UpdateRankingAsync(string user, int score)
        {
            /* intended empty */
            return Task.CompletedTask;
        }

        public IEnumerable<UserRankProjection> FindFriends(string userName)
        {
            var user = Users.SingleOrDefault(x => x.Name == userName);
            if (user is null) return Enumerable.Empty<UserRankProjection>();
            var friends = user.Friends ?? Enumerable.Empty<string>();
            return friends.Join(
                Users,
                f => f,
                x => x.Name,
                (name, user) => new UserRankProjection { Name = name, Ranking = user.Ranking });
        }

        internal static string[] GetFriends(string user) =>
            AllFriends.Except(new[] { user }).ToArray();

        internal static readonly string[] AllFriends = new[]
        {
            Greuceanu,
            Tariceanu,
            Batranu,
            AmAvant,
            Noob,
            NuMiPasa
        };

        internal const string Greuceanu = nameof(Greuceanu);
        internal const string Tariceanu = nameof(Tariceanu);
        internal const string Batranu = nameof(Batranu);
        internal const string AmAvant = nameof(AmAvant);
        internal const string Noob = nameof(Noob);
        internal const string NuMiPasa = nameof(NuMiPasa);

        private static readonly User[] Users = new[]
        {
            new User (Greuceanu, 2000, GetFriends(Greuceanu)),
            new User (Tariceanu, 1800, GetFriends(Tariceanu)),
            new User (Batranu, 1600, GetFriends(Batranu)),
            new User (AmAvant, 1530, GetFriends(AmAvant)),
            new User (Noob, 1450, GetFriends(Noob)),
            new User (NuMiPasa, -100, GetFriends(NuMiPasa)),
        };
    }
}
