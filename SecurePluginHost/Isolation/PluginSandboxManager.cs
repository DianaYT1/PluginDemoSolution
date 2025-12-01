using System;
using System.Collections.Generic;
using System.IO;

namespace SecurePluginHost.Sandboxing
{

    public class PluginSandboxManager
    {
        private readonly string _runnerPath;
        private readonly List<int> _activeSandboxPids = new List<int>();

        public PluginSandboxManager(string runnerPath)
        {
            _runnerPath = runnerPath;
        }

        public string RunPlugin(string pluginPath, string input)
        {
            if (!File.Exists(pluginPath))
                throw new FileNotFoundException("Plugin DLL not found", pluginPath);

            var startInfo = new RunnerProcessStartInfo(_runnerPath, pluginPath, input);

            Console.WriteLine($"[PluginSandboxManager] Launching plugin runner: {pluginPath}");

            var result = startInfo.Run();

            _activeSandboxPids.Add(startInfo.ProcessId);

            return result;
        }

        public void Cleanup()
        {
            Console.WriteLine("[PluginSandboxManager] Cleanup sandbox processes (simulated).");
            _activeSandboxPids.Clear();
        }
    }
}
