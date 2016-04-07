using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ConDep.Node.Model;
using Microsoft.Win32;

namespace ConDep.Node.Controllers.Install
{
    public class MsiController : ApiController
    {
        public HttpResponseMessage Get(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Package name null or empty"));
            }

            UninstallRegKey package;

            if (PackageExist(RegKeys, packageName, out package)) return Request.CreateResponse(HttpStatusCode.OK, new {
                Package = package,
                Links = new[]
            {
                new Link { Href = Url.Request.RequestUri.ToString(), Rel = ApiRels.Self, Method = "GET" }
            }
            });

            return Request.CreateResponse(HttpStatusCode.NotFound, new {
                    Package = package,
                    Links = new[]
                {
                    new Link { Href = Url.Request.RequestUri.ToString(), Rel = ApiRels.Self, Method = "GET" },
                    new Link { Href = ApiUrls.Install.MsiUriTemplate(Url, packageName), Rel = ApiRels.InstallMsiFromUriTemplate, Method = "POST" },
                    new Link { Href = ApiUrls.Install.MsiFileTemplate(Url, packageName), Rel = ApiRels.InstallMsiFromFileTemplate, Method = "POST" }
                }
                });
        }

        public HttpResponseMessage Post(string packageName, Uri packageUri)
        {
            UninstallRegKey package;
            if (PackageExist(RegKeys, packageName, out package))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Found, "Package already installed"));
            }

            string filePath = DownloadPackage(packageUri);
            var result = InstallPackage(filePath);
            return Request.CreateResponse(result.Success ? HttpStatusCode.Created : HttpStatusCode.InternalServerError, result);
        }

        public HttpResponseMessage Post(string packageName, string fileLocation)
        {
            UninstallRegKey package;
            if (PackageExist(RegKeys, packageName, out package))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Found, "Package already installed"));
            }

            var result = InstallPackage(fileLocation);
            return Request.CreateResponse(result.Success ? HttpStatusCode.Created : HttpStatusCode.InternalServerError, result);
        }

        private InstallationResult InstallPackage(string filePath)
        {
            var installResult = new InstallationResult { StartedUtc = DateTime.UtcNow };
            var installLogFile = Path.GetTempFileName();
            try
            {
                var startInfo = new ProcessStartInfo("msiexec.exe")
                {
                    Arguments = string.Format("/q /i {0} /l* {1}", filePath, installLogFile),
                    Verb = "RunAs",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                var process = Process.Start(startInfo);
                process.WaitForExit();

                installResult.ExitCode = process.ExitCode;

                if (process.ExitCode == 0)
                {
                    installResult.Success = true;
                    var message = process.StandardOutput.ReadToEnd();
                    installResult.Message = message;
                }
                else
                {
                    installResult.Success = false;
                    var errorMessage = process.StandardError.ReadToEnd();
                    installResult.ErrorMessage = errorMessage;
                }

                installResult.Log = File.ReadAllText(installLogFile);
                return installResult;
            }
            finally
            {
                if(File.Exists(filePath)) File.Delete(filePath);
                if(File.Exists(installLogFile)) File.Delete(installLogFile);
            }

        }

        private string DownloadPackage(Uri packageUri)
        {
            var fileName = Path.GetTempFileName();
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(packageUri, fileName);
            }
            return fileName;
        }

        private IEnumerable<RegistryKey> RegKeys
        {
            get
            {
                return new List<RegistryKey>
                {
                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"),
                    Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"),
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall")
                };  
            }
        }

        private bool PackageExist(IEnumerable<RegistryKey> keys, string packageName, out UninstallRegKey package)
        {
            package = null;

            if (keys == null) return false;
            
            foreach (var key in keys)
            {
                if (key != null)
                {
                    package = key.GetSubKeyNames()
                        .Select(subKey => new UninstallRegKey(key, subKey))
                        .FirstOrDefault(uninstall => uninstall.DisplayName.Equals(packageName));
                }

                if (package != null) return true;
            }
            return false;
        }
    }

    public class InstallationResult
    {
        public DateTime StartedUtc { get; set; }
        public int ExitCode { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string Log { get; set; }
        public bool Success { get; set; }
    }

    public class UninstallRegKey
    {
        public UninstallRegKey(RegistryKey uninstallKey, string keyName)
        {
            DisplayName = "";
            using (var key = uninstallKey.OpenSubKey(keyName, false))
            {
                if (key == null) return;

                var displayName = key.GetValue("DisplayName");
                if (displayName != null) DisplayName = displayName.ToString();

                var version = key.GetValue("DisplayVersion");
                if (version != null) DisplayVersion = version.ToString();

                var uninstallString = key.GetValue("UninstallString");
                if (uninstallString != null) UninstallString = uninstallString.ToString();
            }
        }

        public string UninstallString { get; set; }

        public string DisplayVersion { get; set; }

        public string DisplayName { get; set; }
    }
}