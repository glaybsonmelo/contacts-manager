using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilter
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await next();
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("mm-DD-yyyy HH:mm");
        }
    }
}



