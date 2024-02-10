using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CRUDExample.Filters.ResourceFilters
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private bool _isDisabled;
        public FeatureDisabledResourceFilter(bool disabled = true)
        {
            _isDisabled = disabled;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (_isDisabled)
                //context.Result = new NotFoundResult();
                context.Result = new StatusCodeResult(501); // Not implemented;
            else
                await next();
        }
    }
}
