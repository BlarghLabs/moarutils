using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MoarUtils.filters {
  public class AccessDeniedAuthorizeAttribute : AuthorizeAttribute {
    public override void OnAuthorization(AuthorizationContext filterContext) {
      base.OnAuthorization(filterContext);
      if (!filterContext.HttpContext.User.Identity.IsAuthenticated) {
        filterContext.Result = new RedirectResult("~/Account/Login");
        return;
      }

      if (filterContext.Result is HttpUnauthorizedResult) {
        filterContext.Result = new RedirectResult("~/Account/Denied");
      }
    }
  }
}
