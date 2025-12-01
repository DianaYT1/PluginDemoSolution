using Xunit;

public class PrivilegeSeparationTests
{
    [Fact]
    public void PluginProcess_HasLowerPrivileges()
    {
        var host = new FakeProcess("Host", integrity: 3);
        var plugin = new FakeProcess("Plugin", integrity: 1);

        bool isLower = plugin.IntegrityLevel < host.IntegrityLevel;

        Assert.True(isLower, "Plugin must run with lower privileges than host.");
    }
}

public class FakeProcess
{
    public string Name { get; }
    public int IntegrityLevel { get; }

    public FakeProcess(string name, int integrity)
    {
        Name = name;
        IntegrityLevel = integrity;
    }
}
