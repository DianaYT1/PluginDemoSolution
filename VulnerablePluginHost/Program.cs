using System;
using PluginContractsTest; 

namespace VulnerablePluginHost
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Vulnerable Plugin Host ===");

            PluginLoader.LoadPlugins();

            Console.WriteLine("Host is running. Type 'exit' to quit.");

            string command = null;

            while (true)
            {
                command = Console.ReadLine();

                if (command == null)
                {
                    break;
                }

                if (command == "exit")
                    break;

                Console.WriteLine("You typed: " + command);
            }

        }
    }
}
