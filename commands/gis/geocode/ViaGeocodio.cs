using MoarUtils.enums;
using MoarUtils.Model;
using MoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace moarutils.utils.gis.geocode {
  //curl "https://api.geocod.io/v1.4/geocode?q=1109+N+Highland+St%2c+Arlington+VA&api_key=YOUR_API_KEY"

  public static class ViaGeocodio {
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Coordinate c,
      string address,
      string key,
      WebProxy wp = null
    ) {
      c = new Coordinate { g = Geocoder.Geocodio };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        if (string.IsNullOrEmpty(address)) {
          status = $"address required";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        //note: ersi puts "0" in china, maybe ignore this one input?
        if (address.Equals("0")) {
          status = $"0 is not properly geocoded by this provider";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        //if (address.Length > 200) {
        //  LogIt.W("address was > 200 length, truncating: " + address);
        //  address = address.Substring(0, 200);
        //}

        var rgx = new Regex(@"[^\w\s]*");
        var addressCheck = rgx.Replace(address.Trim(), "").Trim();
        if (string.IsNullOrWhiteSpace(addressCheck)) {
          status = $"address had no numbers or letters: " + address.Trim();
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        var uea = HttpUtility.UrlEncode(address.Trim());
        //geocode?q=1109+N+Highland+St%2c+Arlington+VA&api_key=YOUR_API_KEY"
        var resource = $"geocode?api_key={key}&limit=1&q=" + uea; // + "&f=pjson&forStorage=false&maxLocations=1";
        var client = new RestClient("https://api.geocod.io/v1.4/");
        var request = new RestRequest(
          resource: resource,
          method: Method.GET
        );
        if (wp != null) {
          client.Proxy = wp;
        }
        var response = client.Execute(request);

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

        if ((json.results == null) || (json.results.Count == 0)) {
          //maybe look at: json.error.code.Value
          LogIt.W(content);
          status = $"results was null";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        var location = json.results[0].location;
        if (location != null) {
          var lng = Convert.ToDecimal(location.lat.Value);
          var lat = Convert.ToDecimal(location.lng.Value);
          if ((lat != 0) && (lng != 0)) {
            c.lng = lng;
            c.lat = lat;
          }
        }
        var precision = json.results[0].accuracy_type.Value;
        if (precision != null) {
          c.precision = precision;
        }

        if (c.lat == 0 || c.lng == 0) {
          LogIt.W("here");
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





