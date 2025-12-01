using System;
using SecureSandboxRunner.Execution;

namespace SecureSandboxRunner
{
    public class SandboxExecutor
    {
        private readonly string _pluginPath;
        private readonly ExecutionEngine _engine;
        private readonly RestrictedEnvironment _env;

        public SandboxExecutor(string pluginPath)
        {
            _pluginPath = pluginPath ?? throw new ArgumentNullException(nameof(pluginPath));
            _engine = new ExecutionEngine(AppDomain.CurrentDomain.FriendlyName); 
            _env = RestrictedEnvironment.CreateDefault();
        }

        public string Execute(string input)
        {
            return _engine.ExecuteIsolated(_pluginPath, input, _env);
        }
    }
}
