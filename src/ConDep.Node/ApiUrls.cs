using System.Web.Http.Routing;

namespace ConDep.Node
{
    public static class ApiUrls
    {
        public static string Home(UrlHelper url) { return url.Link("Api", new { controller = "home" }); }

        public static class Sync
        {
            public static string Home(UrlHelper url) { return url.Link("Api", new { controller = "sync" }); }
            public static string Directory(UrlHelper url) { return url.Link("Sync", new { controller = "Directory" }); }
            public static string DirectoryTemplate(UrlHelper url) { return url.Link("Sync", new { controller = "Directory" }) + "?path={0}"; }
            public static string FileTemplate(UrlHelper url) { return url.Link("Sync", new { controller = "File" }) + "?path={0}"; }
            public static string WebApp(UrlHelper url, string webSite, string webApp) { return url.Link("Iis", new { controller = "Iis", siteName = webSite }) + "?appName=" + webApp + " &path={0}"; }
        }

        public static class Iis
        {
            public static string IisTemplate(UrlHelper url) { return url.Link("Iis", new { controller = "Iis" }) + "/{website}/?appName={webapp}"; }
            public static string WebApp(UrlHelper url, string webSite, string webApp) { return url.Link("Iis", new { controller = "Iis", siteName = webSite }) + "?appName=" + webApp + "&path={0}"; }
            //public static string WebSite(UrlHelper url) { return url.Link("Iis", new { controller = "WebSite" }); }
        }

        public static class Install
        {
            public static string MsiTemplate(UrlHelper url) { return url.Link("Install", new { controller = "Msi" }) + "/{packageName}"; }
            public static string MsiUriTemplate(UrlHelper url, string pName) { return url.Link("Install", new { controller = "Msi", packageName = pName }) + "?packageUri={0}"; }
            public static string MsiFileTemplate(UrlHelper url, string pName) { return url.Link("Install", new { controller = "Msi", packageName = pName }) + "?packageFile={0}"; }
            public static string CustomTemplate(UrlHelper url) { return url.Link("Install", new { controller = "Custom" }) + "/{packageName}"; }
            public static string CustomUriTemplate(UrlHelper url, string pName) { return url.Link("Install", new { controller = "Custom", packageName = pName }) + "?packageUri={0}&packageParams={1}"; }
            public static string CustomFileTemplate(UrlHelper url, string pName) { return url.Link("Install", new { controller = "Custom", packageName = pName }) + "?packageFile={0}&packageParams={1}"; }
        }
    }

    public static class ApiRels
    {
        public static string Self = "self";
        public static string Sync = "http://www.con-dep.net/rels/sync";
        public static string DirTemplate = "http://www.con-dep.net/rels/sync/dir_template";
        public static string FileTemplate = "http://www.con-dep.net/rels/sync/file_template";
        public static string Directory = "http://www.con-dep.net/rels/sync/directory";
        public static string File = "http://www.con-dep.net/rels/sync/file";
        public static string FileSyncTemplate = "http://www.con-dep.net/rels/sync/file_sync_template";
        public static string WebAppTemplate = "http://www.con-dep.net/rels/iis/web_app_template";
        public static string IisTemplate = "http://www.con-dep.net/rels/iis_template";
        public static string InstallMsiTemplate = "http://www.con-dep.net/rels/install/msi_template";
        public static string InstallMsiFromFileTemplate = "http://www.con-dep.net/rels/install/msi_file_template";
        public static string InstallMsiFromUriTemplate = "http://www.con-dep.net/rels/install/msi_uri_template";
        public static string InstallCustomTemplate = "http://www.con-dep.net/rels/install/custom_template";
        public static string InstallCustomFromFileTemplate = "http://www.con-dep.net/rels/install/custom_file_template";
        public static string InstallCustomFromUriTemplate = "http://www.con-dep.net/rels/install/custom_uri_template";
    }
}