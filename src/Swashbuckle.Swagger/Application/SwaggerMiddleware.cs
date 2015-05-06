using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing.Template;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace Swashbuckle.Application
{
    public class SwaggerMiddleware
    {
        private readonly static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new SwaggerDocumentContractResolver(),
        };

        private readonly RequestDelegate _next;
        private readonly Func<HttpRequest, string> _rootUrlResolver;
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly TemplateMatcher _matcher;

        public SwaggerMiddleware(
            RequestDelegate next,
            Func<HttpRequest, string> rootUrlResolver,
            ISwaggerProvider swaggerProvider,
            string routeTemplate)
        {
            _next = next;
            _rootUrlResolver = rootUrlResolver;
            _swaggerProvider = swaggerProvider;

            _matcher = new TemplateMatcher(TemplateParser.Parse(routeTemplate), new Dictionary<string, object>());
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != "GET")
            {
                // TODO handle HEAD/OPTIONS
                await _next(context);
                return;
            }

            // Trim off the leading `/`
            var values = _matcher.Match(context.Request.Path.ToUriComponent().Substring(1));
            if (values == null)
            {
                // Route doesn't match.
                await _next(context);
                return;
            }

            var apiVersion = (string)values["apiVersion"];

            var rootUrl = _rootUrlResolver(context.Request);
            var swagger = _swaggerProvider.GetSwagger(rootUrl, apiVersion);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            using (var writer = new StreamWriter(context.Response.Body, encoding, bufferSize: 1024, leaveOpen: true))
            {
                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, swagger, typeof(SwaggerDocument));
            }
        }
    }
}
