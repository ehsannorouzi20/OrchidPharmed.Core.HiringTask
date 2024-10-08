using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace OrchidPharmed.Core.HiringTask.API.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class UniformResultMiddleware
    {
        private readonly RequestDelegate _next;

        public UniformResultMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var originalBodyStream = httpContext.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;
                await _next(httpContext);
                httpContext.Response.Body = originalBodyStream;
                if (httpContext.Response.StatusCode == StatusCodes.Status200OK)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                    var customResponse = new Structure.APIResponse
                    {
                        ResultObject = !string.IsNullOrEmpty(responseBodyText) ? JsonConvert.DeserializeObject(responseBodyText) : null,
                        ErrorFlag = false,
                        ExtraCommands = null,
                        Refused = false,
                        ResultCode = httpContext.Response.StatusCode,
                        ResultText = null,
                        ResultTextAlias = null,
                        TokenExpired = false
                    };
                    var jsonResponse = JsonConvert.SerializeObject(customResponse);
                    await httpContext.Response.WriteAsync(jsonResponse);
                }
                else
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                    var customResponse = new Structure.APIResponse
                    {
                        ResultObject = null,
                        ErrorFlag = true,
                        ExtraCommands = null,
                        Refused = false,
                        ResultCode = httpContext.Response.StatusCode,
                        ResultText = responseBodyText,
                        ResultTextAlias = responseBodyText,
                        TokenExpired = false
                    };
                    var jsonResponse = JsonConvert.SerializeObject(customResponse);
                    await httpContext.Response.WriteAsync(jsonResponse);
                }
            }
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class UniformResultMiddlewareExtensions
{
    public static IApplicationBuilder UseUniformResultMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<OrchidPharmed.Core.HiringTask.API.Middleware.UniformResultMiddleware>();
    }
}

