using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IExceptionHandling
    {
        public Task HandleAsync(HttpContext context, Exception exception, IWebHostEnvironment env);
    }
}


