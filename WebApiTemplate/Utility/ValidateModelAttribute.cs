using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Validate the model state and return a 400 on a model failure before invoking the controller action.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var content = new HttpError(
                    actionContext.ModelState,
                    actionContext.RequestContext.IncludeErrorDetail);
                
                var result = new NegotiatedContentResult<HttpError>(
                    HttpStatusCode.BadRequest,
                    content,
                    (ApiController)actionContext.ControllerContext.Controller);
                
                actionContext.Response = await result.ExecuteAsync(cancellationToken);
            }
            
            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}
