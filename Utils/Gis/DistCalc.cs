using MoarUtils.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MoarUtils.Utils.Gis {
  public static class DistCalc {
    //Distances Between Points

    // Calculate Distance in Milesdouble
    //d = GeoCodeCalc.CalcDistance(47.8131545175277, -122.783203125, 42.0982224111897, -87.890625);
    // Calculate Distance in Kilometersdouble
    //d = GeoCodeCalc.CalcDistance(47.8131545175277, -122.783203125, 42.0982224111897, -87.890625, GeoCodeCalcMeasurement.Kilometers);


    public enum GeoCodeCalcMeasurement : int { Miles = 0, Kilometers = 1 }
    public const double EarthRadiusInMiles = 3956.0;
    public const double EarthRadiusInKilometers = 6367.0;
    public static double ToRadian(double val) { return val * (Math.PI / 180); }
    public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }
    /* [bs] removed cuz was double, and we use decimal
    /// <summary>
    /// Calculate the distance between two geocodes. Defaults to using Miles.
    /// </summary>
    private static double CalcDistance(double lat1, double lng1, double lat2, double lng2){
        return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
    }
    */

    /// <summary>
    /// Calculate the distance between two geocodes. Defaults to using Miles.
    /// </summary>
    public static decimal CalcDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2) {
      double dResult = CalcDistance(Convert.ToDouble(lat1), Convert.ToDouble(lng1), Convert.ToDouble(lat2), Convert.ToDouble(lng2), GeoCodeCalcMeasurement.Miles);
      return Convert.ToDecimal(dResult);
    }

    /// <summary>
    /// Calculate the distance between two geocodes.
    /// </summary>
    public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m) {
      double radius = DistCalc.EarthRadiusInMiles;
      if (m == GeoCodeCalcMeasurement.Kilometers) {
        radius = DistCalc.EarthRadiusInKilometers;
      }
      return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
    }

    //Geocoder.us
    //http://geocoder.us/help/utility.shtml
    //http://geocoder.us/service/distance?lat1=38&lat2=39&lng1=-122&lng2=-123  
    //15 seconds throtttle
    //http://username:password@geocoder.us/member/service/distance?lat1=38&lat2=39&lng1=-122&lng2=-123
    //http://geocoder.us/member/account
    //$50 for 20,000 queries

    //Some French Site
    //http://www.lacosmo.com/ortho/ortho.html

    public static List<Coordinate> GetItemsWithinDistance(List<Coordinate> loc, Coordinate c, decimal allowableDistanceInMiles) {
      return GetItemsWithinDistance(loc, c, allowableDistanceInMiles, -1);
    }
    public static List<Coordinate> GetItemsWithinDistance(List<Coordinate> loc, Coordinate c, decimal allowableDistanceInMiles, int iOnlyReturnTheClosestX) {
      //If iOnlyReturnTheClosestX == -1, then do NOT limit results
      if (iOnlyReturnTheClosestX == -1) {
        iOnlyReturnTheClosestX = loc.Count;
      }

      List<Coordinate> loc1 = new List<Coordinate>();
      SortedList slResults = new SortedList();

      foreach (var c1 in loc) {
        decimal dDistanceCalculated = CalcDistance(c.lat, c.lng, c1.lat, c1.lng);
        if (dDistanceCalculated <= allowableDistanceInMiles) {
          loc1.Add(c1);
        }
      }

      return loc1;
    }


    public static void GetOneItemWithinShortestDistance(List<Coordinate> loc, Coordinate c, out Coordinate cResult, out decimal distanceResult) {
      cResult = null;
      distanceResult = 0;

      foreach (var c1 in loc) {
        decimal dDistanceCalculated = CalcDistance(c.lat, c.lng, c.lat, c.lng);

        if (cResult == null) {
          //First in list
          cResult = c1;
          distanceResult = dDistanceCalculated;
        } else if (dDistanceCalculated < distanceResult) {
          cResult = c1;
          distanceResult = dDistanceCalculated;
        }
      }
    }

    public static bool IsInAnyCoverageArea(Coordinate c, List<Polygon> lop) {
      foreach (var p in lop) {
        if (IsPointInPoly(p, c)) {
          return true;
        }
      }
      return false;
    }

    public static bool IsPointInPoly(Polygon p, Coordinate c) {
      if (!IsPointInPoly(p.loc, c)) {
        //was not in priary poly
        return false;
      }else{
        //If in outer poly, then confirm not in inner poly
        foreach (var ep in p.exclusionaryPolygons) {
          if (IsPointInPoly(ep.loc, c)) {
            //was in both primary but also exclusionary
            return false;
          }
        }
        //was in primary but not exclusonary
        return true;
      }
    }

    public static bool IsPointInPoly(List<Coordinate> loc, Coordinate c) {
      bool bResult = false;

      //Minus 1 bc 1 pt is listed twice (could otherwsie accomplish w/ give me distnct)
      int iNumberOfVerticies = (loc.Count == 0) ? /* no poly creted yet */ 0 : loc.Count - 1;

      //Do Work
      int i, j;
      for (i = 0, j = iNumberOfVerticies - 1; i < iNumberOfVerticies; j = i++) {
        if (((loc[i].lat > c.lat) != (loc[j].lat > c.lat))
          && (c.lng < (loc[j].lng - loc[i].lng) * (c.lat - loc[i].lat) / (loc[j].lat - loc[i].lat) + loc[i].lng)) {

          bResult = !bResult;
        }
      }

      return bResult;
    }
  }
}
