using System.Web;

namespace MoarUtils.Utils.Web {
  public class IsAndroidDevice {
    public static bool Execute(HttpRequestBase hrb) {
      var userAgent = hrb.UserAgent.ToLower();
      return (
        userAgent.Contains("android")
      );
    }
  }
}


