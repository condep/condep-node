using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Security.Certificates;

namespace ConDep.Node
{
    public class CertificateHandler
    {
        private static string APP_ID = "{4eac5b46-986d-4ae6-b12f-74d7779e19ec}";

        public static X509Certificate2 ConfigureSslCert(string url)
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
                return cert;
            }
            finally
            {
                certStore.Close();
            }
        }

        public static void BindCertificateToIpAndPort(X509Certificate2 cert, string url)
        {
            var uri = new Uri(url);
            var certInfo = HttpApiSslCert.QuerySslCertificateInfo(new IPEndPoint(0, uri.Port));
            if (certInfo != null) HttpApiSslCert.DeleteCertificateBinding(new[] { new IPEndPoint(0, uri.Port) });
            HttpApiSslCert.BindCertificate(new IPEndPoint(0, uri.Port), cert.GetCertHash(), StoreName.My, new Guid(APP_ID));
        }
        public static bool CertificateBoundToIpAndPort(string url, byte[] certHash)
        {
            var uri = new Uri(url);
            var certInfo = HttpApiSslCert.QuerySslCertificateInfo(new IPEndPoint(0, uri.Port));

            if (certInfo == null) return false;

            return certInfo.Hash.SequenceEqual(certHash);
        }

        private static X509Certificate2 AddCertificate(X509Store certStore)
        {
            var builder = new X509CertBuilder("E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", CertStrength.bits_2048);
            var cert = builder.MakeCertificate(Guid.NewGuid().ToString(), "E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", "node.condep.io", 12);
            certStore.Add(cert);
            return cert;
        }

        private static void CleanOldCerts(X509Store certStore)
        {
            var certCol = certStore.Certificates.Find(X509FindType.FindBySubjectName, "node.condep.io", false);
            foreach (var cert in certCol)
            {
                var expiryThreshold = cert.NotAfter.AddDays(-7);
                if (DateTime.UtcNow > expiryThreshold)
                {
                    certStore.Remove(cert);
                }
            }
        }
        private static bool CertificateExist(X509Store certStore, out X509Certificate2 cert)
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
    }
}