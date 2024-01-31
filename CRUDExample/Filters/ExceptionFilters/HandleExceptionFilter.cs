using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ExceptionFilters
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _hostEnvironment;
        public HandleExceptionFilter(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }
        public void OnException(ExceptionContext context)
        {
            if (_hostEnvironment.IsDevelopment()) { }
                context.Result = new ContentResult() { Content = context.Exception.Message, StatusCode = 500 };
        }
    }
}
