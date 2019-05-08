using MoarUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoarUtils.Utils.Web {

  public class GetPublicIp {
    /// <summary>
    /// account for possbility of ELB sheilding the public ip
    /// </summary>
    /// <returns></returns>
    public static string Execute(bool elbIsInUse = true) {
      try {
#if DEBUG
#else
#endif
        //LogIt.D(string.Join("|", new List<object> { HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Headers["X-Forwarded-For"], HttpContext.Current.Request.Headers["REMOTE_ADDR"] }));
        if ((HttpContext.Current == null) || (HttpContext.Current.Request == null)) {
          return null;
        }
        
        var ip = HttpContext.Current.Request.UserHostAddress;
        if (HttpContext.Current.Request.Headers["X-Forwarded-For"] != null) {
          ip = HttpContext.Current.Request.Headers["X-Forwarded-For"];
          //LogIt.D(ip + "|X-Forwarded-For");
        } else if (HttpContext.Current.Request.Headers["REMOTE_ADDR"] != null) {
          ip = HttpContext.Current.Request.Headers["REMOTE_ADDR"];
          //LogIt.D(ip + "|REMOTE_ADDR");
        }

        //ip = string.IsNullOrEmpty(ip) ? ip : ip.Split()
        return ip;
      } catch (Exception ex) {
        LogIt.E(ex);
      }
      return null;
    }
  }
}