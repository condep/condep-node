using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading;
using System.Web.Http.SelfHost;
using Org.BouncyCastle.Security.Certificates;

namespace ConDep.Node
{
    partial class NodeService : ServiceBase
    {
        private readonly string _url;
        private HttpSelfHostServer _server;
        private string APP_ID = "{4eac5b46-986d-4ae6-b12f-74d7779e19ec}";

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
            ConfigureSslCert(_url);
            var config = HttpConfigHandler.CreateConfig(_url);
            _server = new HttpSelfHostServer(config);
             _server.OpenAsync().Wait();
            EventLog.WriteEntry("ConDepNode started", EventLogEntryType.Information);
        }

        private void ConfigureSslCert(string url)
        {
            var certStore = new X509Store(StoreLocation.LocalMachine);

            try
            {
                certStore.Open(OpenFlags.ReadWrite);

                CleanOldCerts(certStore);

                X509Certificate2 cert;

                if (!CertificateExist(certStore, out cert))
                {
                    cert = AddCertificate(certStore);
                }

                if (!CertificateBoundToIpAndPort(url, cert.GetCertHash()))
                {
                    BindCertificateToIpAndPort(cert, url);
                }
            }
            finally
            {
                certStore.Close();
            }
        }

        private void BindCertificateToIpAndPort(X509Certificate2 cert, string url)
        {
            var uri = new Uri(url);
            var certInfo = HttpApiSslCert.QuerySslCertificateInfo(new IPEndPoint(0, uri.Port));
            if(certInfo != null) HttpApiSslCert.DeleteCertificateBinding(new[] { new IPEndPoint(0, uri.Port) });
            HttpApiSslCert.BindCertificate(new IPEndPoint(0, uri.Port), cert.GetCertHash(), StoreName.My, new Guid(APP_ID));
        }

        private bool CertificateBoundToIpAndPort(string url, byte[] certHash)
        {
            var uri = new Uri(url);
            var certInfo = HttpApiSslCert.QuerySslCertificateInfo(new IPEndPoint(0, uri.Port));

            if (certInfo == null) return false;

            return certInfo.Hash.SequenceEqual(certHash);
        }

        private X509Certificate2 AddCertificate(X509Store certStore)
        {
            var builder = new X509CertBuilder("E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", CertStrength.bits_2048);
            var cert = builder.MakeCertificate(Guid.NewGuid().ToString(), "E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", "node.condep.io", 12);
            certStore.Add(cert);
            return cert;
        }

        private void CleanOldCerts(X509Store certStore)
        {
            var certCol = certStore.Certificates.Find(X509FindType.FindBySubjectName, "node.condep.io", false);
            foreach (var cert in certCol)
            {
                if (cert.NotAfter <= DateTime.Now.AddDays(-7)) certStore.Remove(cert);
            }
        }

        private bool CertificateExist(X509Store certStore, out X509Certificate2 cert)
        {
            cert = null;
            var certCol = certStore.Certificates.Find(X509FindType.FindBySubjectName, "node.condep.io", false);
            if (certCol.Count == 0) return false;

            if (certCol.Count > 1)
            {
                throw new CertificateException("More than one node certificate found.");
            }

            cert = certCol[0];
            return true;
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
