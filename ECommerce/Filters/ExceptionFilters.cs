using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.Filters
{
    public class ExceptionFilters : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilters> logger;

        public ExceptionFilters(ILogger<ExceptionFilters> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
