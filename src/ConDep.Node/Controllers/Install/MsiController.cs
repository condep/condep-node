using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ConDep.Node.Model;
using Microsoft.Win32;

namespace ConDep.Node.Controllers.Install
{
    [RoutePrefix("api/install/msi")]
    public class MsiController : ApiController
    {
        [Route("{packageName}")]
        public HttpResponseMessage Get(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Package name null or empty");
            }

            using(var key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (PackageExist(key, packageName)) return Request.CreateResponse(HttpStatusCode.OK, new Link{});
            }

            using (var key = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (PackageExist(key, packageName)) return Request.CreateResponse(HttpStatusCode.OK);
            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (PackageExist(key, packageName)) return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        private bool PackageExist(RegistryKey key, string packageName)
        {
            return key != null && key.GetSubKeyNames()
                .Select(subKey => new UninstallRegKey(key, subKey))
                .Any(uninstall => uninstall.DisplayName.Equals(packageName));
        }
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
            }
        }

        public string DisplayName { get; set; }
    }
}