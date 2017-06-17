using System;

namespace MoarUtils.Utils.measurements {
  public class MeasurementMath {
    public static Decimal? CubicFeetToCubicMeters(Decimal? cubicFeet) {
      if (!cubicFeet.HasValue) {
        return null;
      }
      var result = cubicFeet.Value / 35.3146667M;
      return result;
    }
    public static Decimal? CubicMetersToCubicFeet(
      Decimal? cubicMeters,
      bool roundToZeroDecimals = true
    ) {
      if (!cubicMeters.HasValue) {
        return null;
      }
      var result = cubicMeters.Value * 35.3146667M;
      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }

    public static Decimal? KilogramsToPounds(
      Decimal? kg,
      bool roundToZeroDecimals = true
    ) {
      if (!kg.HasValue) {
        return null;
      }
      var result = kg.Value * 2.20462262M;
      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }

    public static Decimal? PoundsToKilograms(Decimal? lb) {
      if (!lb.HasValue) {
        return null;
      }
      var result = lb.Value / 2.20462262M;
      return result;
    }

  }
}
