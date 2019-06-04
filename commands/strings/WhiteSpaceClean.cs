using System.Text.RegularExpressions;
using System.Web;

namespace MoarUtils.commands.strings {
  public class WhiteSpaceClean {
    public static string Execute(string orig) {
      string result = Regex.Replace(HttpUtility.HtmlDecode(orig), @"\t|\n|\r", " ");
      result = Regex.Replace(result, @"\s+", " ").Trim();
      return result;
    }
  }
}
