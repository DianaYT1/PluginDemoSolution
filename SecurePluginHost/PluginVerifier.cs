    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using PluginContractsTest;

    namespace SecurePluginHost.Security
    {
        public static class PluginVerifier
        {
            private static readonly string TrustedPluginDirectory =
                Path.Combine(AppContext.BaseDirectory, "plugins_trusted");

            private static readonly Version MinSupportedVersion = new Version(1, 0, 0, 0);

            public static PluginVerificationResult Verify(string path)
            {
                if (!File.Exists(path))
                    return PluginVerificationResult.Fail("Файлът не съществува.");

                if (!IsInTrustedDirectory(path))
                    return PluginVerificationResult.Fail("Плъгинът не е в доверена директория.");

                if (!IsSigned(path))
                    return PluginVerificationResult.Fail("Плъгинът НЕ е подписан.");

                var assemblyName = SafeGetAssemblyName(path);
                if (assemblyName == null)
                    return PluginVerificationResult.Fail("Не може да се прочете AssemblyName.");

                if (assemblyName.Version < MinSupportedVersion)
                    return PluginVerificationResult.Fail($"Неподдържана версия: {assemblyName.Version}");

                if (!ContainsValidPluginType(path))
                    return PluginVerificationResult.Fail("Няма тип, който имплементира IPlugin.");

                return PluginVerificationResult.Success();
            }
            private static bool IsInTrustedDirectory(string dllPath)
            {
                string full = Path.GetFullPath(dllPath);
                string trusted = Path.GetFullPath(TrustedPluginDirectory);

                return full.StartsWith(trusted, StringComparison.OrdinalIgnoreCase);
            }

            private static bool IsSigned(string dllPath)
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(dllPath);
                    return name.GetPublicKeyToken()?.Any() == true; 
                }
                catch
                {
                    return false;
                }
            }

            private static AssemblyName SafeGetAssemblyName(string dllPath)
            {
                try
                {
                    return AssemblyName.GetAssemblyName(dllPath);
                }
                catch
                {
                    return null;
                }
            }

            private static bool ContainsValidPluginType(string dllPath)
            {
                try
                {
                    var asm = Assembly.LoadFile(dllPath);

                    return asm.GetTypes().Any(t =>
                        typeof(IPlugin).IsAssignableFrom(t) &&
                        t.IsClass &&
                        !t.IsAbstract);
                }
                catch
                {
                    return false;
                }
            }
        }

        public class PluginVerificationResult
        {
            public bool Ok { get; }
            public string ErrorMessage { get; }

            private PluginVerificationResult(bool ok, string msg = "")
            {
                Ok = ok;
                ErrorMessage = msg;
            }

            public static PluginVerificationResult Success() =>
                new PluginVerificationResult(true);

            public static PluginVerificationResult Fail(string msg) =>
                new PluginVerificationResult(false, msg);
        }
    }
