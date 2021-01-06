using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HttpCli.Server.Services
{
    public class CliStderrQueue
    {
        public Channel<string> Queue { get; } = Channel.CreateUnbounded<string>();
    }
}
