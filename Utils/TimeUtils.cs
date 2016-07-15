using System;

namespace MoarUtils.Utils {
  public class TimeUtils {
    public static string GetDisplayTime(int seconds) {
      var s =
        ((TimeSpan.FromSeconds((int)seconds).Hours == 0) ? "" : (TimeSpan.FromSeconds((int)seconds).Hours + "h "))
        + TimeSpan.FromSeconds((int)seconds).Minutes + "m "
        + TimeSpan.FromSeconds((int)seconds).Seconds + "s";
      return s;
    }
  }
}
