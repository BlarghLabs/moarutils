using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoarUtils.Utils.Web {
  public class ConvertUrlsToLinks {
    public static string Execute(string content) {
      //string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
      var regex = new Regex(
        "(?<!(?:href='|<a[^>]*>))(http|https|ftp|ftps)://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?",
        RegexOptions.IgnoreCase
      );

      MatchCollection matches = regex.Matches(content);
      for (var i = matches.Count - 1; i >= 0; i--) {
        var newURL = "<a href='" + matches[i].Value + "' title='open in new tab' target='_blank' >" + matches[i].Value + "</a>";
        content = content.Remove(matches[i].Index, matches[i].Length).Insert(matches[i].Index, newURL);
      }

      return content;
    }
  }
}




