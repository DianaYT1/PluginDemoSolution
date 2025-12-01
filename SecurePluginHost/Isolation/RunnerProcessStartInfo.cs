using System;
using System.Diagnostics;
using System.IO;

namespace SecurePluginHost.Sandboxing
{
    public class RunnerProcessStartInfo
    {
        public string RunnerPath { get; }
        public string PluginPath { get; }
        public string Input { get; }
        public int ProcessId { get; private set; }

        public RunnerProcessStartInfo(string runnerPath, string pluginPath, string input)
        {
            RunnerPath = runnerPath;
            PluginPath = pluginPath;
            Input = input;
        }

        public string Run()
        {
            if (!File.Exists(RunnerPath))
                throw new FileNotFoundException("Sandbox runner not found", RunnerPath);

            var psi = new ProcessStartInfo
            {
                FileName = RunnerPath,
                Arguments = "\"" + PluginPath + "\" \"" + Input + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(psi))
            {
                if (process == null)
                    throw new InvalidOperationException("Failed to start sandbox runner process.");

                ProcessId = process.Id;

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("[RunnerProcessStartInfo] Runner error: " + error);
                }

                return output;
            }
        }
    }
}
