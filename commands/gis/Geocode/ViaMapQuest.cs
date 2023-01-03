using MoarUtils.enums;
using MoarUtils.Model;
using MoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Web;

//http://www.mapquestapi.com/geocoding/v1/address?key=KEY_HERE&location=lancaster%20pa
//http://www.mapquestapi.com/geocoding/v1/address?key=KEY_HERE&callback=renderOptions&inFormat=kvp&outFormat=json&location=Lancaster,PA

//API Info:

//Rate Limiting

namespace moarutils.utils.gis.geocode {
  public static class ViaMapQuest {
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Coordinate c,
      string address,
      string key,
      WebProxy wp = null
    ) {
      c = new Coordinate { g = Geocoder.MapQuest };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        if (!string.IsNullOrEmpty(address) && !address.Equals("0")) {
          var uea = HttpUtility.UrlEncode(address.Trim());
          var client = new RestClient("https://www.mapquestapi.com/");
          //var client = new RestClient("https://developer.mapquest.com/");
          //var client = new RestClient("http://www.mapquestapi.com/");
          var request = new RestRequest(
            //resource: "/geocoding/v1/address?key=" + key + "&location=" + uea,
            resource: "/geocoding/v1/address?key=" + key + "&location=" + uea,
            method: Method.Get
          );
          //if (wp != null) {
          //  client.Proxy = wp;
          //}
          var response = client.ExecuteAsync(request).Result;

          if (response.ErrorException != null) {
            status = $"response had error exception: {response.ErrorException.Message}";
            hsc = HttpStatusCode.BadRequest;
            return;
          }
          if (response.StatusCode != HttpStatusCode.OK) {
            status = $"StatusCode was {response.StatusCode}";
            hsc = HttpStatusCode.BadRequest;
            return;
          }
          if (string.IsNullOrWhiteSpace(response.Content)) {
            status = $"content was empty";
            hsc = HttpStatusCode.BadRequest;
            return;
          }

          var content = response.Content;
          dynamic json = JObject.Parse(content);
          var sc = (json.info.statuscode == null) ? -1 : json.info.statuscode.Value;
          if (sc != 0) {
            status = (json.info.messages == null)
              ? ""
              : json.info.messages[0].Value
            ;
            hsc = HttpStatusCode.BadRequest;
            return;
          }

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
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        status = $"unexpected error";
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex);
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          address,
          c,
        }, Formatting.Indented));
      }
    }
  }
}