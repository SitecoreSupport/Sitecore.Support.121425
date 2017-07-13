using Sitecore.Forms.Mvc;
using Sitecore.Pipelines;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sitecore.Support.Forms.Mvc.Routes
{
    public class InitializeRoutes
    {
        public virtual void Process(PipelineArgs args)
        {
            InitializeRoutes.RegisterClientEventController();
        }

        private static void RegisterClientEventController()
        {
            RouteCollectionExtensions.MapRoute(RouteTable.Routes, Sitecore.Forms.Mvc.Constants.Routes.ClientEvent, "clientevent/{action}", new
            {
                controller = "ClientEvent",
                action = "Process"
            });
            RouteCollectionExtensions.MapRoute(RouteTable.Routes, Sitecore.Forms.Mvc.Constants.Routes.Form, "form/{action}", new
            {
                controller = "Form",
                action = "Process"
            }, new string[]
            {
                "Sitecore.Support.Forms.Mvc.Controllers"
            });
        }
    }
}
