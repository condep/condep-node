using System;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using static System.Double;

namespace ConDep.Node.Tests
{
    [TestFixture]
    public class CertificateHandlerTests
    {
        [Test]
        public void ConfigureSslCert_IfCertExpiresWithin7Days_Renewed()
        {
            DeleteConDepCertificates();
            try
            {
                var certificate = CreateCertificate(5);
                AddToStore(certificate);
                CertificateHandler.ConfigureSslCert("https://localhost:4444/ConDepNode/");
                var certificatesFromStore = GetCertificates();
                var expiryDateOfCertificate = certificatesFromStore[0].NotAfter;
                var expectedExpiryDateOfCertificate = expiryDateOfCertificate.AddDays(365);
                Assert.That(Math.Abs((expectedExpiryDateOfCertificate - expiryDateOfCertificate).TotalDays - 365) < Epsilon);
                Assert.That(certificatesFromStore.Count == 1);
            }
            finally
            {
                DeleteConDepCertificates();
            }
        }

        [Test]
        public void ConfigureSslCert_IfCertDoesNotExpireWithin7Days_NotRenewed()
        {
            DeleteConDepCertificates();
            try
            {
                var certificate = CreateCertificate(8);
                AddToStore(certificate);
                CertificateHandler.ConfigureSslCert("https://localhost:4444/ConDepNode/");
                var certificatesFromStore = GetCertificates();
                var certificateFromStore = certificatesFromStore[0];
                Assert.AreEqual(certificate.Thumbprint, certificateFromStore.Thumbprint);
            }
            finally
            {
                DeleteConDepCertificates();
            }
        }

        [Test]
        public void ConfigureSslCert_CertHasExpiredOutsideRenewThreshold_Renewed()
        {
            DeleteConDepCertificates();
            try
            {
                var certificate = CreateCertificate(-8);
                AddToStore(certificate);
                CertificateHandler.ConfigureSslCert("https://localhost:4444/ConDepNode/");
                var certificatesFromStore = GetCertificates();
                var renewedCertificate = certificatesFromStore[0];
                Assert.AreNotEqual(renewedCertificate.Thumbprint, certificate.Thumbprint);
            }
            finally
            {
                DeleteConDepCertificates();
            }
        }

        private static X509Certificate2Collection GetCertificates()
        {
            var certStore = new X509Store(StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            var certificatesFromStore = certStore.Certificates.Find(X509FindType.FindBySubjectName, "node.condep.io", false);
            certStore.Close();
            return certificatesFromStore;
        }

        private static void DeleteConDepCertificates()
        {
            X509Store certStore = null;
            try
            {
                certStore = new X509Store(StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadWrite);
                var certificates = certStore.Certificates.Find(X509FindType.FindBySubjectName, "node.condep.io", false);
                foreach (var cert in certificates)
                {
                    certStore.Remove(cert);
                }
            }
            finally
            {
                certStore?.Close();
            }
        }

        private static void AddToStore(X509Certificate2 certificate)
        {
            X509Store certStore = null;
            try
            {
                certStore = new X509Store(StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadWrite);
                certStore.Add(certificate);

            }
            finally
            {
                certStore?.Close();
            }
        }

        private static X509Certificate2 CreateCertificate(int numberOfDaysValid)
        {
            var builder = new X509CertBuilder("E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", CertStrength.bits_2048);
            var cert = builder.MakeCertificate(Guid.NewGuid().ToString(), "E=condep@condep.io, CN=node.condep.io, O=ConDep, L=Bergen, C=NO", "node.condep.io", numberOfDaysValid);
            return cert;
        }

    }
}