using System.ServiceProcess;

namespace ConDep.Node
{
    class Program
    {
        static void Main(string[] args)
        {
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
    }
}