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
  //http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/find?text=1700 Penny ave washingtn dc&f=pjson&forStorage=false&maxLocations=1

  public static class ViaEsri {
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Coordinate c,
      string address,
      WebProxy wp = null
    ) {
      c = new Coordinate { g = Geocoder.Esri };
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

        if (address.Length > 200) {
          LogIt.W("address was > 200 length, truncating: " + address);
          address = address.Substring(0, 200);
        }

        var rgx = new Regex(@"[^\w\s]*");
        var addressCheck = rgx.Replace(address.Trim(), "");
        if (string.IsNullOrWhiteSpace(addressCheck)) {
          status = $"address had no numbers or letters";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        var uea = HttpUtility.UrlEncode(address.Trim());
        var resource = "/arcgis/rest/services/World/GeocodeServer/find?text=" + uea + "&f=pjson&forStorage=false&maxLocations=1";
        var client = new RestClient("https://geocode.arcgis.com/"); //https?
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

        if ((json.locations == null) || (json.locations.Count == 0)) {
          //maybe look at: json.error.code.Value
          LogIt.W(content);
          status = $"location result was null";
          hsc = HttpStatusCode.BadRequest;
          return;
        }


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





