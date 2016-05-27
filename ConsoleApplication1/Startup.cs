namespace Microshaoft
{
    using Owin;
    using System.Web.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Net;
    public class Startup
    {
        // This code configures Web API contained in the class Startup, which is additionally specified as the type parameter in WebApplication.Start
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for Self-Host
            var config = new HttpConfiguration();

            var httpListener = (HttpListener)
                                        appBuilder
                                            .Properties[typeof(HttpListener).FullName];
            httpListener
                .AuthenticationSchemes =
                       //AuthenticationSchemes
                         //     .IntegratedWindowsAuthentication
                        ////    | AuthenticationSchemes.Negotiate
                        //    | 
                            //|
                            AuthenticationSchemes.Ntlm;

            //appBuilder.UseCors()


            config
                .MapHttpAttributeRoutes();
            config
                .Formatters
                .JsonFormatter
                .MediaTypeMappings
                .Add
                    (
                        new QueryStringMapping
                                (
                                    "type"
                                    , "json"
                                    , new MediaTypeHeaderValue("application/json")
                                )
                    );
            config
                .Formatters
                .XmlFormatter
                .MediaTypeMappings
                .Add
                    (
                        new QueryStringMapping
                                (
                                    "type"
                                    , "xml"
                                    , new MediaTypeHeaderValue("application/xml")
                                )
                    );
            config
                .EnableCors();

            
            //config
            //    .Routes
            //    .MapHttpRoute
            //        (
            //            name: "DefaultApi",
            //            routeTemplate: "api/{controller}/{id}",
            //            defaults: new { id = RouteParameter.Optional }
            //        );

            appBuilder.UseWebApi(config);
        }
    }
}