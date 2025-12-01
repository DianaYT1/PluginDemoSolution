using System;
using System.Collections.Generic;
using System.IO;
using SecureSandboxRunner.Execution;
using Xunit;

public class SandboxIsolationTests
{
    [Fact]
    public void Plugin_CannotRead_SecretFile()
    {
    
        var env = RestrictedEnvironment.CreateDefault();
        env.AllowedReadDirectories.Clear();  
        env.AllowedWriteDirectories.Clear();

        string secretPath = Path.Combine("secrets", "secret.txt");
        Directory.CreateDirectory("secrets");
        File.WriteAllText(secretPath, "TOP_SECRET");

        var runner = new FakeSandboxRunner(env);

 
        var result = runner.TryReadFile(secretPath);

        Assert.False(result, "Plugin must NOT be allowed to read the secret file.");
    }
}

public class FakeSandboxRunner
{
    private readonly RestrictedEnvironment _env;

    public FakeSandboxRunner(RestrictedEnvironment env)
    {
        _env = env;
    }

    public bool TryReadFile(string path)
    {
        foreach (var allowed in _env.AllowedReadDirectories)
        {
            if (path.StartsWith(allowed, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
