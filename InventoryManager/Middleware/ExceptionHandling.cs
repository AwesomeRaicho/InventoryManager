using InventoryManager.Core.Interfaces;
using InventoryManager.Core.Exceptions;

namespace InventoryManager.Middleware
{
    public class ExceptionHandling : IExceptionHandling
    {
        public async Task HandleAsync(HttpContext context, Exception exception)
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

            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred."
                });
            }
        }
    }
}



