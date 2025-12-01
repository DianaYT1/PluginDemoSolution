using System.Collections.Generic;

namespace SecureSandboxRunner.Execution
{ 
    public class RestrictedEnvironment
    {
        public int MaxMemoryMb { get; set; } = 100;

        public int MaxCpuSeconds { get; set; } = 2;

        public List<string> AllowedReadDirectories { get; } = new List<string>();
        public List<string> AllowedWriteDirectories { get; } = new List<string>();

        public List<string> BlockedNativeLibraries { get; } = new List<string>();

        public bool NetworkAccessAllowed { get; set; } = false;

        public static RestrictedEnvironment CreateDefault()
        {
            return new RestrictedEnvironment
            {
                MaxMemoryMb = 50,
                MaxCpuSeconds = 1,
                NetworkAccessAllowed = false
            };
        }
    }
}
