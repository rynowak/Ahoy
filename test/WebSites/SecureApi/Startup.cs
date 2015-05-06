using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using SecureApi.Swagger;

namespace SecureApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwagger(s =>
            {
                s.SwaggerGenerator(opts =>
                {
                    opts.SecurityDefinitions.Add("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = "http://petstore.swagger.io/api/oauth/dialog",
                        Scopes = new Dictionary<string, string>
                        {
                            { "read", "read access" },
                            { "write", "write access" }
                        }
                    });

                    opts.OperationFilter<AssignSecurityRequirements>();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI();

            // TOOD: Figure out oauth middleware to validate token
            //app.UseOAuthBearerAuthentication(opts =>
            //{
            //});
        }
    }
}
