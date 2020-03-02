using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    class Program
    {

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        static void Main()
        {
            Program p = new Program();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "credentials.json");
            p.Run();
            Console.In.ReadLine();
        } 
        public void Run()
        {
            // First lets copy in our plugin directory.
            if (Directory.Exists("plugin"))
            {
                Directory.Delete("plugin", true);
            }

            // Lets create our plugin directory.
            Directory.CreateDirectory("plugin");
            // Copy the project.

            DirectoryCopy("..\\..\\..\\Plugin\\bin\\Debug", "plugin", true);

            var evidence = new Evidence(AppDomain.CurrentDomain.Evidence);

            var setupInfo = new AppDomainSetup()
            {
                ApplicationBase = Path.Combine(Directory.GetCurrentDirectory(), "plugin"),
                ConfigurationFile = Path.Combine(Directory.GetCurrentDirectory(), "plugin", "Plugin.dll.config")
            };

            var pluginDomain = AppDomain.CreateDomain("Core", evidence, setupInfo);
            var assemblyLoader = (PluginLoader)pluginDomain.CreateInstanceAndUnwrap(typeof(PluginLoader).Assembly.FullName, typeof(PluginLoader).FullName);
            assemblyLoader.LoadFrom(Path.Combine(setupInfo.ApplicationBase, "Plugin.dll"));
            var pluginObj = pluginDomain.CreateInstanceAndUnwrap("Plugin", "Plugin.Plugin") as IPlugin;
            pluginObj.Start();
            pluginObj.Stop();
            AppDomain.Unload(pluginDomain);
            try { 
                Directory.Delete("plugin", true); } 
            catch (Exception e) 
            {
                Console.Out.WriteLine("failed to delete plugin directory");
                Console.Out.WriteLine(e.Message);
            }

        }
    }
}
