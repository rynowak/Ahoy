using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.StaticFiles;

namespace Swashbuckle.Application
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSwaggerUI(
            this IApplicationBuilder app,
            string requestPath = "/swagger/ui")
        {
            var options = new FileServerOptions();

            // Only serve files starting with this path
            options.RequestPath = requestPath;

            // Automatically turn '/' into 'index.html'
            options.DefaultFilesOptions.DefaultFileNames.Clear();
            options.DefaultFilesOptions.DefaultFileNames.Add("index.html");

            // Automatically map content types (should include everything we need).
            options.StaticFileOptions.ContentTypeProvider = new FileExtensionContentTypeProvider();

            // Use embedded resources instead of a real file-system.
            options.FileProvider = new EmbeddedFileProvider(
                typeof(ApplicationBuilderExtensions).GetTypeInfo().Assembly,
                "Swashbuckle.SwaggerUi.bower_components.swagger_ui.dist");

            app.UseFileServer(options);
        }
    }
}