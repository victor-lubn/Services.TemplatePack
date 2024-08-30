using System.Collections.Generic;
using System.Threading.Tasks;
using Lueben.Microservice.CircuitBreaker;
using Lueben.Microservice.DurableFunction.Exceptions;

namespace Lueben.Templates.DurableMicroservice.Function.Extensions
{
    public static class CircuitBreakerStateCheckerExtensions
    {
        public static async Task ThrowIfCircuitBreakerIsOpen(this ICircuitBreakerStateChecker circuitBreakerStateChecker, string[] ids)
        {
            if (await circuitBreakerStateChecker.IsCircuitBreakerInOpenState(new List<string>(ids)))
            {
                throw new EventDataProcessFailureException($"Circuit breaker is open for one of the dependencies: {string.Join(',', ids)}");
            }
        }
    }
}
