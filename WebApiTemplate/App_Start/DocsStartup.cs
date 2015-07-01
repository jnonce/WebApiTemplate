using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using jnonce.WebApi.VersionedRouting;
using Owin;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Augment the HttpConfiguration with routes for Swagger docs
    /// </summary>
    internal class DocsStartup : IStartup
    {
        private HttpConfiguration config;
        private IAppBuilder app;

        public DocsStartup(IAppBuilder app, HttpConfiguration config)
        {
            this.app = app;
            this.config = config;
        }

        public Task Configuration(Func<Task> next)
        {
            config
                .EnableSwagger(ConfigureSwagger)
                .EnableSwaggerUi(ConfigureSwaggerUI);
            return next();
        }


        // Configure the Swagger UI presentation
        private static void ConfigureSwaggerUI(SwaggerUiConfig swaggerUi)
        {
            swaggerUi.EnableDiscoveryUrlSelector();
        }

        // Configure Swagger's document generation
        private void ConfigureSwagger(SwaggerDocsConfig swagger)
        {
            ConfigureSwaggerFromXmlComments(swagger);

            swagger.MultipleApiVersions(ApiDescriptionMatchesVersion, ConfigureSwaggerVersions);

            // Add optional parameters to every API.
            // This is needed to let callers know about parameters which aren't directly exposed
            // on a controller or action.
            swagger.OperationFilter(
                () => new AddedParametersSwaggerOperationFilter(
                    new Parameter
                    {
                        name = "x-correlation-id",
                        description = "Request correlation id",
                        type = "string",
                        @in = "header",
                        required = false
                    },
                    new Parameter
                    {
                        name = "api-version",
                        description = "API version to access",
                        type = "string",
                        @in = "header",
                        required = false
                    })
                );
        }

        private static void ConfigureSwaggerFromXmlComments(SwaggerDocsConfig swagger)
        {
            var codebaseUri = new Uri(typeof(DocsStartup).Assembly.CodeBase);
            string path = Path.ChangeExtension(codebaseUri.LocalPath, "xml");

            if (File.Exists(path))
            {
                swagger.IncludeXmlComments(path);
            }
        }

        private void ConfigureSwaggerVersions(VersionInfoBuilder versionInfoBuilder)
        {
            versionInfoBuilder.Version("4.0", "July 2014 API version");
            versionInfoBuilder.Version("2.0", "Jan 2014 API version");
            versionInfoBuilder.Version("1.0", "Original API version");
        }

        private bool ApiDescriptionMatchesVersion(System.Web.Http.Description.ApiDescription apiDesc, string targetApiVersion)
        {
            object constraintObj;
            if (!apiDesc.Route.Constraints.TryGetValue(ApiVersionRouteConstraint.ConstraintKey, out constraintObj))
            {
                return true;
            }

            var constraint = constraintObj as ApiVersionRouteConstraint;
            return (constraint == null)
                || constraint.Match(targetApiVersion);
        }
    }
}
