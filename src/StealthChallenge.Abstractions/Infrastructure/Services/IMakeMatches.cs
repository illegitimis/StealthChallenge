using System.Threading.Tasks;
using System.Collections.Generic;

namespace StealthChallenge.Abstractions.Infrastructure.Services
{
    public interface IMakeMatches
    {
        Task<IEnumerable<string>> Match(string userName);
    }
}
