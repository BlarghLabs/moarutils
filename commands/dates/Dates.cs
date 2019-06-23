using System;

namespace MoarUtils.Utils {
  public class Dates {
    public static String TimeZoneName(DateTime dt) {
      String sName = TimeZone.CurrentTimeZone.IsDaylightSavingTime(dt)
          ? TimeZone.CurrentTimeZone.DaylightName
          : TimeZone.CurrentTimeZone.StandardName;

      String sNewName = "";
      String[] sSplit = sName.Split(new char[] { ' ' });
      foreach (String s in sSplit)
        if (s.Length >= 1)
          sNewName += s.Substring(0, 1);

      return sNewName;
    }

    public static DateTime FromUnixTime(long unixTime) {
      var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      return epoch.AddSeconds(unixTime);
    }
  }
}
