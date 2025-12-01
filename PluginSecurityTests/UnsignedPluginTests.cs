using System;
using System.IO;
using SecurePluginHost.Security;
using Xunit;

public class UnsignedPluginTests
{
    [Fact]
    public void UnsignedPlugin_IsBlocked()
    {
        string fakeDll = "fakePlugin.dll";
        File.WriteAllText(fakeDll, "dummy");

        var verificationResult = PluginVerifier.Verify(fakeDll);

        Assert.False(verificationResult.Ok);

        File.Delete(fakeDll);
    }
}

