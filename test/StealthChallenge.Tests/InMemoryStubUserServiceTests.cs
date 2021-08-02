namespace StealthChallenge.Tests
{
    using StealthChallenge.Abstractions.Domain.Services;
    using StealthChallenge.Abstractions.Infrastructure.Services;
    using StealthChallenge.Domain.Services;
    using StealthChallenge.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using static StealthChallenge.Domain.Services.InMemoryStubUserService;

    public class InMemoryStubUserServiceTests
    {
        [Fact]
        public void AllExceptSelf()
        {
            var strings = GetFriends(Greuceanu);
            Assert.Equal(AllFriends.Length - 1, strings.Length);
        }

        [Fact]
        public async Task Projection()
        {
            IUserService userService = new InMemoryStubUserService();
            var friends = await userService.FindFriendsAsync(AmAvant);
            Assert.Equal(5, friends.Count());
        }
    }

    public class RankingRangeMatchmakerTests
    {
        private readonly IMakeMatches _matchMaker;

        public RankingRangeMatchmakerTests()
        {
            _matchMaker = new RankingRangeMatchmaker(
                new InMemoryStubUserService(),
                100);
        }

        [Theory]
        [InlineData(Greuceanu, 0)]
        [InlineData(Tariceanu, 0)]
        [InlineData(Batranu, 1)]
        [InlineData(AmAvant, 2)]
        [InlineData(Noob, 1)]
        [InlineData(NuMiPasa, 0)]
        public async Task FriendsCount(string user, int expectedNoFriends)
        {
            var friends = await _matchMaker.Match(user);
            Assert.Equal(expectedNoFriends, friends.Count());
        }
    }
}
