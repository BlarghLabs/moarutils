using RestSharp.Extensions.MonoHttp;
using System;
using System.Text.RegularExpressions;

namespace MoarUtils.Utils {
  public class Strings {
    public static string WhiteSpaceClean(string orig) {
      string result = Regex.Replace(HttpUtility.HtmlDecode(orig), @"\t|\n|\r", " ");
      result = Regex.Replace(result, @"\s+", " ").Trim();
      return result;
    }
    public static string RemoveHtmlComments(string orig) {
      return Regex.Replace(orig, "<!--.*?-->", String.Empty, RegexOptions.Singleline);
    }
    
    public static string AnyTwoToOne(string orig) {
      RegexOptions options = RegexOptions.None;
      Regex regex = new Regex(@"[ ]{2,}", options);
      return regex.Replace(orig, @" ").Trim();

    }

    public static string CondenseWhiteSpace(string input) {
      string result = input;
      while (result.IndexOf("  ") != -1) {
        result = result.Replace("  ", " ");
      }
      return result;
    }

    //This is case sensitive
    public static string StripStrFromBegAndEnd(string orig, string toRemove) {
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

    public static string Truncate(string value, int maxChars) {
      return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
    }
  }
}
