﻿using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Routing;
using Link = ConDep.Node.Model.Link;

namespace ConDep.Node.Controllers
{
    public class HomeController : ApiController
    {
        public IEnumerable<Link> Get()
         {
             return new List<Link>
                        {
                            new Link { Href = ApiUrls.Home(Url), Rel = ApiRels.Self, Method = "GET"},
                            new Link { Href = ApiUrls.Sync.Home(Url), Rel = ApiRels.Sync, Method = "GET"},
                            new Link { Href = ApiUrls.Sync.DirectoryTemplate(Url), Rel = ApiRels.DirTemplate , Method = "GET"},
                            new Link { Href = ApiUrls.Sync.FileTemplate(Url), Rel = ApiRels.FileTemplate, Method = "GET"},
                            new Link { Href = ApiUrls.Iis.IisTemplate(Url), Rel = ApiRels.IisTemplate, Method = "GET"},
                            new Link { Href = ApiUrls.Install.MsiTemplate(Url), Rel = ApiRels.InstallMsiTemplate, Method = "GET"},
                            new Link { Href = ApiUrls.Install.CustomTemplate(Url), Rel = ApiRels.InstallCustomTemplate, Method = "GET"}
                        };
         }
    }
}