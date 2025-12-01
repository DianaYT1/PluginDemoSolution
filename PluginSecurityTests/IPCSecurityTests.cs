using Xunit;

public class IPCSecurityTests
{
    [Fact]
    public void UnauthorizedClient_CannotConnect()
    {
        var server = new FakeNamedPipeServer();

        string unauthorizedUser = "UnknownUser";

        bool allowed = server.CanConnect(unauthorizedUser);

        Assert.False(allowed);
    }
}

public class FakeNamedPipeServer
{
    private readonly string[] _allowedUsers = new[] { "SecureHostUser" };

    public bool CanConnect(string user)
    {
        foreach (string u in _allowedUsers)
        {
            if (string.Equals(user, u, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
