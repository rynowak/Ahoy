using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.ApiExplorer;
using Microsoft.Framework.DependencyInjection;
using Swashbuckle.Swagger;
using Swashbuckle.Application;
using VersionedApi.Versioning;
using VersionedApi.Swagger;

namespace VersionedApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwagger(s =>
            {
                s.SwaggerGenerator(opt =>
                {
                    opt.MultipleApiVersions(
                        new []
                        {
                            new Info { Version = "v1", Title = "API V1" },
                            new Info { Version = "v2", Title = "API V2" }
                        },
                        ResolveVersionSupportByVersionsConstraint);

                    opt.DocumentFilter<AddVersionToBasePath>();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI();
        }
        private static bool ResolveVersionSupportByVersionsConstraint(ApiDescription apiDesc, string version)
        {
            var versionAttribute = apiDesc.ActionDescriptor.ActionConstraints.OfType<VersionsAttribute>()
                .FirstOrDefault();
            if (versionAttribute == null) return true;

            return versionAttribute.AcceptedVersions.Contains(version);
        }
    }
}
