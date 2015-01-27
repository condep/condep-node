using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Web.Http.SelfHost;

namespace ConDep.Node
{
    partial class NodeService : ServiceBase
    {
        private readonly string _url;
        private HttpSelfHostServer _server;

        public NodeService(string url)
        {
            _url = url;
            InitializeComponent();

            this.CanHandlePowerEvent = false;
            this.CanHandleSessionChangeEvent = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = false;
            this.CanStop = true;
            this.EventLog.EnableRaisingEvents = true;
            this.EventLog.Source = "ConDepNode";
            this.EventLog.Log = "Application";

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => EventLog.WriteEntry("Error: " + args.ExceptionObject.ToString(), EventLogEntryType.Error);
        }

        protected override void OnStart(string[] args)
        {
            CertificateHandler.ConfigureSslCert(_url);

            var config = HttpConfigHandler.CreateConfig(_url);
            _server = new HttpSelfHostServer(config);
             _server.OpenAsync().Wait();
            EventLog.WriteEntry("ConDepNode started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
            EventLog.WriteEntry("ConDepNode stopped", EventLogEntryType.Information);
        }

        public void Start(string[] args)
        {
            OnStart(args);
            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
