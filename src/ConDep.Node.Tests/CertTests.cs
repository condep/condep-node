using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace ConDep.Node.Tests
{
    [TestFixture]
    public class CertTests
    {
        [Test]
        public void TestThat()
        {
            var cert = HttpApiSslCert.QuerySslCertificateInfo(new IPEndPoint(0, 4445));
            //HttpApiSslCert.BindCertificate(new IPEndPoint(0, 4444), new byte[0], StoreName.My, Guid.NewGuid());

            //var cb = new X509CertBuilder("E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", CertStrength.bits_2048);
            //X509Certificate2 cert = cb.MakeCertificate(Guid.NewGuid().ToString(), "E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", "node.condep.io", 1);

            //var certStore = new X509Store(StoreLocation.LocalMachine);
            //certStore.Open(OpenFlags.ReadWrite);
            //certStore.Add(cert);
            //var thumbprint = cert.Thumbprint;
            //certStore.Close();

        }

        [Test]
        public void TestThat_FindCerts()
        {
            var certStore = new X509Store(StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            var certCol = certStore.Certificates.Find(X509FindType.FindBySubjectName, "node.condep.io", false);
            if (certCol.Count == 0)
            {
                
            }
            certStore.Close();
        }

    }
}