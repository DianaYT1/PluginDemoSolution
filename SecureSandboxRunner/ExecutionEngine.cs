using System;
using System.Diagnostics;
using System.IO;

namespace SecureSandboxRunner.Execution
{
    public class ExecutionEngine
    {
        private readonly string _sandboxRunnerPath;

        public ExecutionEngine(string sandboxRunnerPath)
        {
            _sandboxRunnerPath = sandboxRunnerPath; 
        }

        public string ExecuteIsolated(string pluginPath, string input, RestrictedEnvironment env)
        {
            if (!File.Exists(_sandboxRunnerPath))
                throw new FileNotFoundException("SecureSandboxRunner not found");

            var psi = new ProcessStartInfo
            {
                FileName = _sandboxRunnerPath,
                Arguments = $"\"{pluginPath}\" \"{input}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Console.WriteLine("[ExecutionEngine] Launching sandbox runner...");

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
    }
}
