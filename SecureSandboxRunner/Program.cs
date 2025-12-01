using System;
using System.IO;
using PluginContractsTest;

namespace SecureSandboxRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("[Sandbox] Runner started.");

            if (args.Length < 2)
            {
                Console.WriteLine("ERR: Invalid arguments.");
                return 1;
            }

            string pluginPath = args[0];
            string input = args[1];

            if (!File.Exists(pluginPath))
            {
                Console.WriteLine("ERR: Plugin not found.");
                return 2;
            }

            try
            {
                var executor = new SandboxExecutor(pluginPath);
                string output = executor.Execute(input);

                Console.WriteLine(output);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR: " + ex.Message);
                return 3;
            }
        }
    }
}
