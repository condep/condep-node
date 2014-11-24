using System;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConDep.Node
{
    public class HttpConfigHandler
    {
        public static HttpSelfHostConfiguration CreateConfig(string url)
        {
            var uri = new Uri(url);
            var config = new NtlmSelfHostConfiguration(uri)
            {
                TransferMode = TransferMode.Streamed,
                MaxReceivedMessageSize = 2147483648
            };

            var serializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            serializerSettings.Formatting = Formatting.Indented;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            AddRoutes(config);
            return config;
        }

        public static void AddRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("Install", "api/install/{controller}/{packageName}",
                new
                {
                    packageName = RouteParameter.Optional
                });
            config.Routes.MapHttpRoute("Sync", "api/sync/{controller}");
            config.Routes.MapHttpRoute("WebAppSync", "api/sync/webapp/{siteName}", new { controller = "WebApp" });
            config.Routes.MapHttpRoute("Iis", "api/iis/{siteName}",
                new
                {
                    controller = "Iis",
                    siteName = RouteParameter.Optional,
                });
            config.Routes.MapHttpRoute("Api", "api/{controller}", new {controller = "Home"});
        }
    }
}