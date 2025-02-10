using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using InventoryManager.Core.Interfaces;
using System.Text.Json;

namespace InventoryManager.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandling _exceptionHandling;

        public ExceptionHandlerMiddleware(RequestDelegate next, IExceptionHandling exceptionHandling)
        {
            _next = next;
            _exceptionHandling = exceptionHandling;
        }

        public async Task Invoke(HttpContext httpContext, IWebHostEnvironment env)
        {
            try
            {
                await _next(httpContext);
            }catch (Exception ex)
            {
                await _exceptionHandling.HandleAsync(httpContext, ex, env);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}


