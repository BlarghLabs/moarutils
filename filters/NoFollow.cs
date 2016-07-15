using System.Web.Http.Filters;

namespace MoarUtils.filters {
  public class NoFollow : ActionFilterAttribute {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
      actionExecutedContext.Response.Content.Headers.Add("X-Robots-Tag", "none"); //noindex
    }
  }
}
