using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HttpCli.Server.Services
{
    public class CliService
    {
        private Process _process=null!;
        public CliStdoutQueue CliStdoutQueue { get; }
        public CliStderrQueue CliStderrQueue { get; }
        public CliStdinQueue CliStdinQueue { get; }
        public CliService(
            CliStdoutQueue cliStdoutQueue,
            CliStderrQueue cliStderrQueue,
            CliStdinQueue cliStdinQueue)
        {
            CliStdoutQueue = cliStdoutQueue;
            CliStderrQueue = cliStderrQueue;
            CliStdinQueue = cliStdinQueue;
            ResetCli();
            HandleInputAsync().ConfigureAwait(false);
        }
        public void ResetCli()
        {
            if (_process is not null) { _process.Kill(); }
            ProcessStartInfo startInfo = new("cmd")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            _process = new() { StartInfo = startInfo };
            _process.OutputDataReceived += (sender, e) => CliStdoutQueue.Queue.Writer.TryWrite(e.Data!);
            _process.ErrorDataReceived += (sender, e) => CliStderrQueue.Queue.Writer.TryWrite(e.Data!);

            _process.Start();
            _process.StandardInput.AutoFlush = true;
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }
        public void EnqueueInput(string input) => CliStdinQueue.Queue.Writer.TryWrite(input);
        private async Task HandleInputAsync()
        {
            while(await CliStdinQueue.Queue.Reader.WaitToReadAsync())
            {
                string input = await CliStdinQueue.Queue.Reader.ReadAsync();
                _process.StandardInput.WriteLine(input);
            }
        }
    }
}
