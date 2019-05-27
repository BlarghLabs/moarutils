using MoarUtils.enums;
using MoarUtils.Model;
using RestSharp;
using System;
using System.Web;

namespace MoarUtils.Utils.Gis.Geocode {
  //note: ersi puts "0" in china, maybe ignore this one input?

  public class ViaEsri {
    public static Coordinate Execute(string address) {
      var c = new Coordinate { g = Geocoder.Esri };

      try {
        if (!string.IsNullOrEmpty(address) && !address.Equals("0")) {
          var uea = HttpUtility.UrlEncode(address.Trim());

          //http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/find?text=1700 Penny ave washingtn dc&f=pjson&forStorage=false&maxLocations=1
          var client = new RestClient("http://geocode.arcgis.com/");
          var request = new RestRequest("/arcgis/rest/services/World/GeocodeServer/find?text=" + uea + "&f=pjson&forStorage=false&maxLocations=1", Method.GET);
          var response = client.Execute(request);

          if (response.StatusCode != System.Net.HttpStatusCode.OK) {
            LogIt.D(response.StatusCode + "|" + address);
          } else {
            var content = response.Content;
            dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);

            if (json.locations.Count > 0) {
              var geometry = json.locations[0].feature.geometry;
              if (geometry != null) {
                var lng = Convert.ToDecimal(geometry.x.Value);
                var lat = Convert.ToDecimal(geometry.y.Value);
                if ((lat != 0) && (lng != 0)) {
                  c.lng = lng;
                  c.lat = lat;
                }
              }
              var precision = json.locations[0].feature.attributes;
              if (precision != null) {
                var pc = precision.Score.Value;
                c.precision = Convert.ToString(pc);
              }            
            }
          }
        }
      } catch (Exception ex) {
        LogIt.W(address);
        LogIt.E(ex);
      }

      return c;
    }
  }
}

