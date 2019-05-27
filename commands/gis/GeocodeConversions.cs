using System;

namespace MoarUtils.Utils.Gis {
  public static class GeocodeConversions {
    public static double DegreesMinutesSecondsToGeocode(string NSEW, int degrees, int minutes, double seconds) {
      switch (NSEW.ToUpper()) {
        case "N":
        case "E":
          return DegreesMinutesSecondsToGeocode(degrees, minutes, seconds);
        case "S":
        case "W":
          return ((double)-1) * DegreesMinutesSecondsToGeocode(degrees, minutes, seconds);
        default:
          throw new Exception("Invalid NSEW: " + NSEW);
      }
    }

    public static double DegreesMinutesSecondsToGeocode(int degrees, int minutes, double seconds) {
      double geocode = (double)degrees + ((double)minutes / 60) + ((double)seconds / 3600);
      return geocode;
    }

    public static double GeocodeToDegreesMinutesSeconds(double geocode, out int degrees, out int minutes, out double seconds) {
      degrees = Convert.ToInt32(geocode - (geocode % 1));
      minutes = Convert.ToInt32((geocode % 1) * 60);
      seconds = (((geocode % 1) * 60.0) % 1) * 60;

      return geocode;
    }
  }
}
