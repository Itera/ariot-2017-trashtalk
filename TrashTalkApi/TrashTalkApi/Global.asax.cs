using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using TrashTalkApi.Models;
using TrashTalkApi.Repositories;

namespace TrashTalkApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            DocumentDbRepository<StoredTrashCanStatus>.Initialize();

        }
    }
}
