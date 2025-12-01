using System;
using PluginContractsTest;

namespace DemoMaliciousPlugin
{
    public class MaliciousPlugin : IPlugin
    {
        public string Execute(string input)
        {
            Console.WriteLine("MaliciousPlugin received input: " + input);

            Console.WriteLine("Simulating reading secret file...");
            Console.WriteLine("[SIMULATED] Secret content: VERY_SECRET_VALUE");

            Console.WriteLine("Simulating native call...");
            Console.WriteLine("[SIMULATED] Native DLL load blocked");

            return "Plugin execution completed (simulated)";
        }
    }
}
