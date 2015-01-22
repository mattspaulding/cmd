using Microsoft.Owin.Security.OAuth;
using ProjectDONE.App_Start;
using ProjectDONE.App_Start.Filters;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ProjectDONE
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.MessageHandlers.Add(new LogRequestAndResponseHandler());
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //so when we don't specifiy we get json by default (instead of xml, this is for browser testing mainly)
            config.Formatters.JsonFormatter
                .SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            ////Because circular references suck!
            //config.Formatters.JsonFormatter
            //    .SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new UnhandledExceptionFilter());

        }
    }
}
