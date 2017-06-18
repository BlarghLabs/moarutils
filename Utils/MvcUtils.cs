using System.Web;

namespace MoarUtils.Utils {
  public class MvcUtils {
    public static string controllerName {
      get {
        var cn = ((HttpContext.Current == null) || (HttpContext.Current.Request == null))
          ? ""
          : HttpContext.Current.Request.RequestContext.RouteData.Values["Controller"].ToString().ToLower();
        return cn;
      }
    }
    public static string actionName {
      get {
        var an = ((HttpContext.Current == null) || (HttpContext.Current.Request == null))
          ? ""
          : HttpContext.Current.Request.RequestContext.RouteData.Values["Action"].ToString().ToLower();
        return an;
      }
    }

    public static bool isMatch(string controller, string action) {
      return (controllerName == controller) && (actionName == action);
    }
  }
}
