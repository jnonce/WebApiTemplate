using System;
using System.IO;
using System.Web.Http;
using Autofac;
using jnonce.WebApi.VersionedRouting;
using Owin;
using Swashbuckle.Application;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        /// <summary>
        /// Configures the docs.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="container">The container.</param>
        /// <param name="config">The configuration.</param>
        public void ConfigureDocs(
            IAppBuilder app,
            IContainer container,
            HttpConfiguration config)
        {
            if (config.IncludeErrorDetailPolicy == IncludeErrorDetailPolicy.Always)
            {
                config
                    .EnableSwagger(ConfigureSwagger)
                    .EnableSwaggerUi(ConfigureSwaggerUI);
            }
        }

        private static void ConfigureSwaggerUI(SwaggerUiConfig swaggerUi)
        {
            swaggerUi.EnableDiscoveryUrlSelector();
        }

        private void ConfigureSwagger(SwaggerDocsConfig swagger)
        {
            var codebaseUri = new Uri(typeof(Startup).Assembly.CodeBase);
            string path = Path.ChangeExtension(codebaseUri.LocalPath, "xml");

            if (File.Exists(path))
            {
                swagger.IncludeXmlComments(path);
            }

            swagger.MultipleApiVersions(ApiDescriptionMatchesVersion, ConfigureSwaggerVersions);
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
