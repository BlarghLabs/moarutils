using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoarUtils.Utils.Web {
  public class IsIOSDevice {
    public static bool Execute(HttpRequestBase hrb) {
      var userAgent = hrb.UserAgent.ToLower();
      return (userAgent.Contains("iphone;") || userAgent.Contains("ipad;"));
    }
  }
}
