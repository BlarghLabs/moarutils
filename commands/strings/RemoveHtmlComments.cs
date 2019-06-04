using System;
using System.Text.RegularExpressions;

namespace MoarUtils.commands.strings {
  public class RemoveHtmlComments {
    public static string Execute(string orig) {
      return Regex.Replace(orig, "<!--.*?-->", String.Empty, RegexOptions.Singleline);
    }

  }
}
