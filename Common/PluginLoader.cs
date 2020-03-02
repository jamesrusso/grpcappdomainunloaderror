using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Common
{
    public class PluginLoader : MarshalByRefObject
    {
        public PluginLoader()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(args.Name + ".dll");
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.Out.WriteLine($"CORE: {args.LoadedAssembly.FullName} {args.LoadedAssembly.Location}");
        }

        public void LoadFrom(string path)
        {
            
            Assembly.LoadFrom(path);
        }

    }
}

