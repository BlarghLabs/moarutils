using System.Web;

namespace MoarUtils.commands.web {
  public class IsAndroidRestUserAgent {
    public static bool Execute(HttpRequestBase hrb) {
      var userAgent = hrb.UserAgent.ToLower();
      return (
        userAgent.Contains("okhttp")
      );
    }
  }
}


