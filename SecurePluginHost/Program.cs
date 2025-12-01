using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using SecurePluginHost.Security;

using PluginContractsTest;
using SecureSandboxRunner;

namespace SecurePluginHost
{
    class Program
    {
        private static readonly string TrustedPluginsPath =
            Path.Combine(AppContext.BaseDirectory, "plugins_trusted");

        private static readonly string SecretsPath =
            Path.Combine(AppContext.BaseDirectory, "secrets", "secret.txt");

        static void Main(string[] args)
        {
            Console.WriteLine("=== Secure Plugin Host ===");

            EnsureDirectories();

            string secret = LoadSecretSafely();
            Console.WriteLine("Secret loaded (not shown for security).");

            StartSecureIPC(secret);

            Console.WriteLine("Secure host exiting.");
        }

        private static void EnsureDirectories()
        {
            Directory.CreateDirectory(TrustedPluginsPath);

            string secretsDir = Path.GetDirectoryName(SecretsPath);
            if (!string.IsNullOrEmpty(secretsDir))
                Directory.CreateDirectory(secretsDir);
        }

        private static string LoadSecretSafely()
        {
            if (!File.Exists(SecretsPath))
                File.WriteAllText(SecretsPath, "DEFAULT_SECRET_VALUE");

            return File.ReadAllText(SecretsPath);
        }

        private static void StartSecureIPC(string secret)
        {
            Console.WriteLine("[+] Starting secure IPC...");

            PipeSecurity ps = new PipeSecurity();

            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;

            ps.AddAccessRule(new PipeAccessRule(
                sid,
                PipeAccessRights.ReadWrite,
                AccessControlType.Allow));

            ps.AddAccessRule(new PipeAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                PipeAccessRights.ReadWrite,
                AccessControlType.Deny));

            using (var pipeServer = new NamedPipeServerStream(
                "SecurePluginPipe",
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous,
                4096,
                4096,
                ps))
            {
                Console.WriteLine("[+] Waiting for secure client connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("[+] Client connected.");

                using (var reader = new StreamReader(pipeServer))
                using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                {
                    while (true)
                    {
                        string request = reader.ReadLine();
                        if (request == null)
                            break;

                        if (request == "exit")
                            break;

                        var parts = request.Split(':');
                        if (parts.Length != 2)
                        {
                            writer.WriteLine("ERR: invalid format");
                            continue;
                        }

                        string pluginName = parts[0];
                        string pluginInput = parts[1];

                        string pluginPath = Path.Combine(TrustedPluginsPath, pluginName + ".dll");
                        if (!File.Exists(pluginPath))
                        {
                            writer.WriteLine("ERR: plugin not found");
                            continue;
                        }

                        var verify = PluginVerifier.Verify(pluginPath);
                        if (!verify.Ok)
                        {
                            writer.WriteLine("ERR: plugin rejected - " + verify.ErrorMessage);
                            continue;
                        }

                        Console.WriteLine("[+] Loading plugin '" + pluginName + "' in sandbox...");

                        var sandbox = new PluginSandbox(pluginPath);
                        string response;

                        try
                        {
                            response = sandbox.Execute(pluginInput);
                        }
                        catch (Exception ex)
                        {
                            response = "Plugin execution failed: " + ex.Message;
                        }

                        writer.WriteLine(response);
                    }

                    Console.WriteLine("[+] Client disconnected.");
                }
            }
        }
    }
}
