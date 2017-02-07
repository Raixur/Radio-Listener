using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RadioListener.Services.Radio.Commands
{
    public abstract class BashCommand
    {
        protected void Start(string command)
        {
            var process = new Process
            {
                StartInfo = GetStartInfo(command)
            };
            process.Start();
            process.WaitForExit();
        }

        protected async Task StartAsync(string command)
        {
            var startInfo = GetStartInfo(command);
            await RunAsync(startInfo);
        }

        private static ProcessStartInfo GetStartInfo(string command)
        {
            return new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \" {command} > /dev/null \""
            };
        }

        private static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo)
        {
            return RunAsync(processStartInfo, CancellationToken.None);
        }

        private static async Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
        {
            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<ProcessResults>();

            var standardOutput = new List<string>();
            var standardError = new List<string>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardOutput.Add(args.Data);
                }
                else
                {
                    standardOutputResults.SetResult(standardOutput.ToArray());
                }
            };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardError.Add(args.Data);
                }
                else
                {
                    standardErrorResults.SetResult(standardError.ToArray());
                }
            };

            process.Exited += (sender, args) =>
            {
                tcs.TrySetResult(new ProcessResults(standardOutputResults.Task.Result,
                                                    standardErrorResults.Task.Result));
            };

            using (cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch(InvalidOperationException) { }
            }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (process.Start() == false)
                {
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return await tcs.Task;
            }
        }
    }

    public class ProcessResults
    {
        public ProcessResults(string[] standardOutput, string[] standardError)
        {
            StandardOutput = standardOutput;
            StandardError = standardError;
        }
        public string[] StandardOutput { get; }

        public string[] StandardError { get; }
    }
}