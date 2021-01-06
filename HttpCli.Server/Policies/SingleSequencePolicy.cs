using Polly;
using Polly.Bulkhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpCli.Server.Policies
{
    public class SingleSequencePolicy
    {
        private readonly AsyncBulkheadPolicy _policy = Policy.BulkheadAsync(1, 128);
        public async ValueTask<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action) => await _policy.ExecuteAsync(action);
           
    }
}
