using MoarUtils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoarUtils.Model {
  public class Coordinate {
    public decimal lat;
    public decimal lng;
    public string precision;

    public string title;
    public string desc;
    public string iconUrl;

    public Geocoder g = Geocoder.Unknown;
    public bool fromDbCache = false;

    public string ToString(bool bMakeGoogleMapLink, string sTargetName) {
      string sResult = "";
      try {
        sResult = "Lat=" + lat.ToString() + "|Long=" + lng.ToString() + "|Src=" + g.ToString() + "|Precision=" + precision;

        if (bMakeGoogleMapLink) {
          sTargetName = string.IsNullOrEmpty(sTargetName) ? "_gmaplink" : sTargetName;
          sResult = "<a target='" + sTargetName + "' href='http://maps.google.com/maps?q=" + lat.ToString() + "," + lng.ToString() + "'>" + sResult + "</a>";

          if (g == Geocoder.Yahoo) {
            sTargetName = string.IsNullOrEmpty(sTargetName) ? "_gmaplink" : sTargetName;
            sResult += "|<a target='" + sTargetName + "' href='http://maps.yahoo.com/#mvt=m&lat=" + lat.ToString() + "&lon=" + lng.ToString() + "&tp=1&zoom=14'>Y</a>";
          }
          /*
          if (_eGeocodeSource == GeocodeSource.MapQuest) {
            sTargetName = string.IsNullOrEmpty(sTargetName) ? "_gmaplink" : sTargetName;
            sResult += "|<a target='" + sTargetName + "' href='http://maps.yahoo.com/#mvt=m&lat=" + _latitude.ToString() + "&lon=" + _longitude.ToString() + "&tp=1&zoom=14'>MQ</a>";
          } 
          */
        }
      } catch /* (Exception ex) */ { }

      return sResult;
    }
  }
}