using System;
using System.Text.RegularExpressions;

namespace MoarUtils.Utils {
  public class Urls {
    public static string GetDomain(string url) {
      var domain = "";
      try {
        var uri = new Uri(url);
        domain = !string.IsNullOrEmpty(uri.Host) ? uri.Host : (url.Contains(":") ? url.Substring(0, url.IndexOf(":")) : url);
      } catch { } finally {
        domain = string.IsNullOrEmpty(domain) ? null : domain;
      }
      return domain;
    }
  }
}
