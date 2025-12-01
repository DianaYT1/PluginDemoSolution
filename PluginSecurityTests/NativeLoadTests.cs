
using SecureSandboxRunner.Execution;
using Xunit;

public class NativeLoadTests
{
    [Fact]
    public void Plugin_NativeDllLoad_IsBlocked()
    {
        var env = RestrictedEnvironment.CreateDefault();
        env.BlockedNativeLibraries.Add("kernel32.dll");
        env.BlockedNativeLibraries.Add("user32.dll");

        var security = new NativeLibrarySecurity(env);

        bool allowed = security.IsNativeCallAllowed("kernel32.dll");

        Assert.False(allowed);
    }
}

public class NativeLibrarySecurity
{
    private readonly RestrictedEnvironment _env;

    public NativeLibrarySecurity(RestrictedEnvironment env)
    {
        _env = env;
    }

    public bool IsNativeCallAllowed(string dllName)
    {
        dllName = dllName.ToLowerInvariant();
        return !_env.BlockedNativeLibraries.Contains(dllName);
    }
}
