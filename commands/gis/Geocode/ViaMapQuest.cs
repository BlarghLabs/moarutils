using MoarUtils.enums;
using MoarUtils.Model;
using MoarUtils.Utils;
using RestSharp;
using System;
using System.Web;


//http://dev.virtualearth.net/REST/v1/Locations/US/adminDistrict/locality/addressLine?includeNeighborhood=includeNeighborhood&include=includeValue&maxResults=maxResults&key=BingMapsKey
//http://dev.virtualearth.net/REST/v1/Locations?query=locationQuery&includeNeighborhood=includeNeighborhood&include=includeValue&maxResults=maxResults&key=BingMapsKey
//API Info:

//Rate Limiting

namespace MoarUtils.Utils.Gis.Geocode {
  
  public class ViaMapQuest{
    
    //http://www.mapquestapi.com/geocoding/v1/address?key=KEY_HERE&location=lancaster%20pa
    //http://www.mapquestapi.com/geocoding/v1/address?key=KEY_HERE&callback=renderOptions&inFormat=kvp&outFormat=json&location=Lancaster,PA

    public static Coordinate Execute(string address, string key) {
      var c = new Coordinate { g= Geocoder.MapQuest };

      try {
        if (!string.IsNullOrEmpty(address)) {
          var uea = HttpUtility.UrlEncode(address.Trim());

          var client = new RestClient("http://www.mapquestapi.com/");
          var request = new RestRequest("/geocoding/v1/address?key=" + key + "&location=" + uea, Method.GET);
          var response = client.Execute(request);

          if (response.StatusCode != System.Net.HttpStatusCode.OK) {
            LogIt.W($"status was {response.StatusCode}");
            return c;
          }
          if (string.IsNullOrEmpty(response.Content)) {
            LogIt.W($"content was empty");
            return c;
          }

          var content = response.Content;
          dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
          var sc = (json.info.statuscode == null) ? -1 : json.info.statuscode.Value;

          if (sc != 0) {
            var msg = (json.info.messages == null) ? "" : json.info.messages[0].Value;
            LogIt.W(msg);

            //temp
            //var js = json.results[0].locations[0].ToString();
            //LogIt.D(js);
          } else {
            var lng = Convert.ToDecimal(json.results[0].locations[0].latLng.lng.Value);
            var lat = Convert.ToDecimal(json.results[0].locations[0].latLng.lat.Value);
            var pc = json.results[0].locations[0].geocodeQualityCode.Value; //A1XAX and A3XAX are bad country level precision

            if ((lat != 0) && (lng != 0) && (pc != "A1XAX") && (pc != "A3XAX")) {
              c.lng = lng;
              c.lat = lat;
              c.precision = pc;
              //c.sGeocodePrecision = json.results[0].locations[0].geocodeQuality;
            }
          }
        }
      } catch (Exception ex) {
        LogIt.E(ex);
      }

      return c;
    }
  }
}
