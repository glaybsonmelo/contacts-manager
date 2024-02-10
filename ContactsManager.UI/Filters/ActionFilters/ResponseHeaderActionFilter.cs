using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly string _key;
        private readonly string _value;

        public int Order { get; }
        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            _key = key;
            _value = value;
            Order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = new ResponseHeaderActionFilter(_key, _value, Order);
            return filter;
        }
    }
    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter                                                                             
    {
        private readonly string _key;
        private readonly string _value;
        
        public int Order { get; }

        public ResponseHeaderActionFilter(string key, string value, int order)
        {
            _key = key;
            _value = value;
            Order = order;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Response.Headers[_key] = _value;
            await next();
        }
    }
}
