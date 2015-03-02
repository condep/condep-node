using System;
using System.Reflection;
using System.ServiceProcess;

namespace ConDep.Node
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureAssemblyResolver();
            var url = "https://localhost:4444/ConDepNode/";
            if(args.Length > 0)
            {
                url = args[0];
            }
//#if(DEBUG)
//            var service = new NodeService(url);
//            service.Start(args);
//#else
            ServiceBase.Run(new ServiceBase[] { new NodeService(url) });
//#endif
        }

        private static void ConfigureAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String resourceName = "ConDep.Node.Assemblies." + new AssemblyName(args.Name).Name + ".dll";

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}