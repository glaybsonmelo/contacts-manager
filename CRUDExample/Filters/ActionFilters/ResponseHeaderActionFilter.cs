using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        private readonly string _key;
        private readonly string _value;
        public int Order { get; set; }

        public ResponseHeaderActionFilter(string key, string value, int order)
        {
            _key = key;
            _value = value;
            Order = order;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            context.HttpContext.Response.Headers[_key] = _value;
        }
    }
}
