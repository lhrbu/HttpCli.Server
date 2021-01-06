using HttpCli.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Polly;
using HttpCli.Server.Policies;

namespace HttpCli.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CliController : ControllerBase
    {
        private readonly CliService _cliService;
        private readonly SingleSequencePolicy _singleSequencePolicy;
        public CliController(CliService cliService,
            SingleSequencePolicy singleSequencePolicy)
        { 
            _cliService = cliService;
            _singleSequencePolicy = singleSequencePolicy;
        }

        [HttpPost]
        public void EnqueueInput([FromBody] string input) => _cliService.EnqueueInput(input);

        [HttpGet]
        public async ValueTask<string?> GetAsync() => await
            _singleSequencePolicy.ExecuteAsync(async () =>
                {
                    ChannelReader<string> reader = _cliService.CliStdoutQueue.Queue.Reader;
                    if (reader.Count > 0) { return await reader.ReadAsync(); }
                    else { return null; }
                });

        [HttpDelete]
        public void ResetCli() => _cliService.ResetCli();

    }
}
