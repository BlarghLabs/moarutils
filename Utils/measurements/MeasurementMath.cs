using System;

namespace MoarUtils.Utils.measurements {
  public class MeasurementMath {
    #region volume  
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
    #endregion

    #region weight
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
    #endregion

    #region length 
    public static Decimal? CentimetersToInches(
      Decimal? cm,
      bool roundToZeroDecimals = true
    ) {
      if (!cm.HasValue) {
        return null;
      }
      var result = cm.Value * 0.393701M;
      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }

    public static Decimal? InchesToCentimeters(
      Decimal? inches
    ) {
      if (!inches.HasValue) {
        return null;
      }
      var result = inches.Value / 0.393701M;
      return result;
    }
    #endregion
  }
}
