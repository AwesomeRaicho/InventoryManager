using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryManager.Middleware
{
    public class ExceptionHandling : IExceptionHandling
    {
        public async Task HandleAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
        {
            if(exception is ProductAlreadyExistsException product)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = exception.Message,
                    ProductName = product.ProductName,
                    ProductNumber = product.ProductNumber,
                });

            }else if(exception is ProductTypeIdDoesNotExist productTypeId)
            {
                context.Response.StatusCode= StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = exception.Message,
                    ProductTypeId = productTypeId,
                });
            }else if(exception is ProductDoesNotExist productDoesNotExist)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = exception.Message,
                    ProductId = productDoesNotExist.ProductId,
                });
            }
            else
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new
                {
                    message = env.IsDevelopment() ? exception.Message : "An internal server error occurred.",
                    details = env.IsDevelopment() ? exception.StackTrace : null 
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}



