using StealthChallenge.Abstractions.Infrastructure.Commands;
using System.Threading.Tasks;
using System.IO.Pipelines;

namespace StealthChallenge.Api
{
    public interface IClientCommandStrategy
    {
        public Task HandleAsync(IClientCommand clientCommand, PipeWriter pipeWriter);
    }
}
