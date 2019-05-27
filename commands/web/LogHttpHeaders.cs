using MoarUtils.Utils;
using System;
using System.Web;

namespace MoarUtils.commands.web {
  public class LogHttpHeaders {
    public static void Execute(){
      try {
        foreach (var rh in HttpContext.Current.Request.Headers) {
          LogIt.D(rh.ToString() + "|" + HttpContext.Current.Request.Headers[rh.ToString()]);
        }
      } catch (Exception ex) {
        LogIt.E(ex);
      }
    }
  }
}