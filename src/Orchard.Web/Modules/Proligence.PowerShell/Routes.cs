namespace Proligence.PowerShell
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Orchard.Mvc.Routes;

    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor routeDescriptor in this.GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
                   {
                        new RouteDescriptor
                        {
                            Priority = 20,
                            Route =
                                new Route(
                                "Test/{action}/{id}",
                                new RouteValueDictionary
                                    {
                                       { "area", "Proligence.PowerShell" }, 
                                       { "controller", "Test" },
                                       { "id", UrlParameter.Optional }
                                    }, 
                                new RouteValueDictionary(), 
                                new RouteValueDictionary { { "area", "Proligence.PowerShell" } }, 
                                new MvcRouteHandler())
                        }
                   };
        }
    }
}