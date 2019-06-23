using MoarUtils.commands.logging;
using System;
using System.Globalization;
using System.Linq;

namespace MoarUtils.Utils.i18n {
  public static class LogSupportedCultures {
    public static void Execute() {
      try {
        foreach (var ci in CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures).OrderBy(ci => ci.Name)) {
          LogIt.D(ci.Name + "|" + ci.EnglishName);
        }
      } catch (Exception ex) {
        LogIt.E(ex);
      }
    }
  }
}