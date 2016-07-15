using System.Web.Mvc;

namespace MoarUtils.filters {
  public class EmptyLinesRemover : ActionFilterAttribute {
    public override void OnResultExecuted(ResultExecutedContext filterContext) {
      base.OnResultExecuted(filterContext);
      var response = filterContext.HttpContext.Response;
      response.Filter = new Utils.Streams.RemoveEmptyLineWriter(response.Filter);
    }
  }


}
