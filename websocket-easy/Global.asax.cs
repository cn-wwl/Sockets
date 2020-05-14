 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using websocket_easy.ClientSocket;

namespace websocket_easy
{
    public class WebApiApplication : System.Web.HttpApplication
    { 
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Fleckwebsockt.ServerStart();
            ClientAsynSocket.Init(); 
        }
    }
}
