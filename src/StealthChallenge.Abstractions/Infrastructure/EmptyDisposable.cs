using System;

namespace StealthChallenge.Abstractions.Infrastructure
{
    public sealed class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
            // no dispose op
        }
    }
}