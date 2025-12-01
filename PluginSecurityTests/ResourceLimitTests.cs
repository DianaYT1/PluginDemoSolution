
using SecureSandboxRunner.Execution;
using Xunit;
public class ResourceLimitTests
{
    [Fact]
    public void Plugin_ExceedsMemoryLimit_IsStopped()
    {
        var env = new RestrictedEnvironment
        {
            MaxMemoryMb = 50
        };

        var sandbox = new FakeResourceMonitor(env);

        int usage = 120;

        bool stopped = sandbox.ShouldStopProcess(usage);

        Assert.True(stopped);
    }
}

public class FakeResourceMonitor
{
    private readonly RestrictedEnvironment _env;

    public FakeResourceMonitor(RestrictedEnvironment env)
    {
        _env = env;
    }

    public bool ShouldStopProcess(int memoryUsageMb)
    {
        return memoryUsageMb > _env.MaxMemoryMb;
    }
}
