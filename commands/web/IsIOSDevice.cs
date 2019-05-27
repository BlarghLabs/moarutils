using System.Web;

namespace MoarUtils.commands.web {
  public class IsIOSDevice {
    public static bool Execute(HttpRequestBase hrb) {
      var userAgent = hrb.UserAgent.ToLower();
      return (userAgent.Contains("iphone;") || userAgent.Contains("ipad;") || userAgent.Contains("ipod"));
    }
  }
}
