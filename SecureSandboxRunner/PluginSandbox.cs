using System;
using SecureSandboxRunner.Execution;

namespace SecureSandboxRunner
{
    public class PluginSandbox
    {
        private readonly string _pluginPath;
        private readonly ExecutionEngine _engine;

        public PluginSandbox(string pluginPath)
        {
            if (string.IsNullOrEmpty(pluginPath))
                throw new ArgumentNullException(nameof(pluginPath));

            _pluginPath = pluginPath;
            _engine = new ExecutionEngine(AppDomain.CurrentDomain.FriendlyName);
        }

        public string Execute(string input)
        {
            var env = RestrictedEnvironment.CreateDefault();
            return _engine.ExecuteIsolated(_pluginPath, input, env);
        }
    }
}
