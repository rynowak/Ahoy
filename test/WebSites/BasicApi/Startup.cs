using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;
using Swashbuckle.Swagger;
using Swashbuckle.Application;
using BasicApi.Swagger;

namespace BasicApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwagger(s =>
            {
                s.SwaggerGenerator(c =>
                {
                    c.Schemes = new[] { "http", "https" };
                    c.SingleApiVersion(new Info
                    {
                        Version = "v1",
                        Title = "Swashbuckle Sample API",
                        Description = "A sample API for testing Swashbuckle",
                        TermsOfService = "Some terms ..."
                    });

                    c.OperationFilter<AssignOperationVendorExtensions>();
                });

                s.SchemaGenerator(opt => opt.DescribeAllEnumsAsStrings = true);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
