using System;
using System.Text.RegularExpressions;
using System.Web;

namespace MoarUtils.commands.strings {
  public class StripStrFromBegAndEnd {
    //This is case sensitive
    public static string Execute(string orig, string toRemove) {
      var result = orig.Trim();
      if (toRemove.Length > 0) {
        if (result.Length > 0) {
          if (result.StartsWith(toRemove)) {
            result = result.Substring(toRemove.Length);
          }
        }

        if (result.Length > 0) {
          if (result.EndsWith(toRemove)) {
            result = result.Substring(0, result.Length - toRemove.Length);
          }
        }
      }
      result = result.Trim();
      return result;
    }
  }
}
