using System.Web.Mvc;
using System.Web.Routing;

namespace BookStore
{
    #region "App route"
    public class RouteConfig
    {
        /// <summary>Registers the routes.</summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Authentication", action = "SignIn", id = UrlParameter.Optional }
            );
        }
    }
    #endregion
}
