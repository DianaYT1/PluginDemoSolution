using System;
using System.IO;
using System.Reflection;
using PluginContractsTest;

namespace VulnerablePluginHost
{
    public static class PluginLoader
    {
        private const string PluginsFolder = "plugins";

        public static void LoadPlugins()
        {
            if (!Directory.Exists(PluginsFolder))
            {
                Console.WriteLine("Plugins folder not found.");
                return;
            }

            foreach (var file in Directory.GetFiles(PluginsFolder, "*.dll"))
            {
                try
                {
                    Console.WriteLine($"Loading plugin: {file}");

                    var assembly = Assembly.LoadFrom(file);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface)
                        {
                            var instance = Activator.CreateInstance(type);
                            if (instance is IPlugin plugin)
                            {
                                Console.WriteLine("Plugin executed output: " + plugin.Execute("Hello from host"));
                            }
                            else
                            {
                                throw new InvalidOperationException("Could not create instance of plugin.");
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {file}: {ex.Message}");
                }
            }
        }
    }
}
