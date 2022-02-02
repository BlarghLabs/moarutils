using MoarUtils.commands.strings;
using MoarUtils.enums;
using MoarUtils.Model;
using MoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Web;

namespace moarutils.utils.gis.geocode {
  public static class ViaGoogle {
    private const double m_dThrottleSeconds = .9; //1.725; //0.1; //100ms //Convert.ToDouble(1.725);
    private static DateTime dtLastRequest = DateTime.UtcNow;
    private static Mutex mLastRequest = new Mutex();

    private static string GetUrlSecondPart(string location, string key = null) {
      string locationUrl = "";
      if (!String.IsNullOrEmpty(location)) {
        locationUrl = "address=" + Uri.EscapeDataString(CondenseWhiteSpace.Execute(HttpUtility.UrlEncode(location.Replace("+", " "))));
      }
      return "maps/api/geocode/json?" + locationUrl + "&sensor=false" + (string.IsNullOrEmpty(key) ? "" : $"&key={key}");
    }

    public static Coordinate Execute(
      string address,
      bool useRateLimit,
      WebProxy wp = null,
      int maxTriesIfQueryLimitReached = 1,
      bool throwOnUnableToGeocode = true,
      string key = null
    ) {
      lock (mLastRequest) {
        //Force delay of 1.725 seconds between requests: re: http://groups.google.com/group/Google-Maps-API/browse_thread/thread/906e871bcb8c15fd
        TimeSpan tsDuration;
        bool bRequiredWaitTimeHasElapsed;
        do {
          tsDuration = DateTime.UtcNow - dtLastRequest;
          bRequiredWaitTimeHasElapsed = (tsDuration.TotalSeconds > m_dThrottleSeconds);
          if (!bRequiredWaitTimeHasElapsed) {
            int iMillisecondsToSleep = Convert.ToInt32((m_dThrottleSeconds - tsDuration.TotalSeconds) * Convert.ToDouble(1000));
            Thread.Sleep(iMillisecondsToSleep);
          }
        } while (!bRequiredWaitTimeHasElapsed);

        Execute(
          hsc: out HttpStatusCode hsc,
          status: out string status,
          c: out Coordinate c,
          address: address,
          wp: wp,
          key: key,
          maxTriesIfQueryLimitReached: maxTriesIfQueryLimitReached
        );
        if (hsc != HttpStatusCode.OK) {
          LogIt.E("unable to geocode");
        }
        return c;
      }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Coordinate c,
      string address,
      WebProxy wp = null,
      int maxTriesIfQueryLimitReached = 1,
      string key = null
    ) {
      c = new Coordinate { g = Geocoder.Google };
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        if (string.IsNullOrEmpty(address)) {
          status = $"address required";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        int trys = 1;
        do {
          var client = new RestClient("https://maps.googleapis.com/");
          var request = new RestRequest(
            resource: GetUrlSecondPart(address, key),
            method: Method.GET
            //dataFormat: DataFormat.Json
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
          status = json.status.Value;
          switch (status) {
            case "OK":
              var results = json.results;
#if DEBUG
              Console.WriteLine(results);
#endif
              var lat = json.results[0].geometry.location.lat.ToString();
              lat = string.IsNullOrWhiteSpace(lat)
                ? ""
                : lat
              ;
              var lng = json.results[0].geometry.location.lng.ToString();
              lng = string.IsNullOrWhiteSpace(lng)
                ? ""
                : lng
              ;

              var location_type = json.results[0].geometry.location_type;
              location_type = (location_type == null)
                ? ""
                : location_type
              ;
              c.lat = Convert.ToDecimal(lat);
              c.lng = Convert.ToDecimal(lng);
              c.precision = location_type;
              hsc = HttpStatusCode.OK;
              return;
            case "UNKNOWN_ERROR":
              if (trys < maxTriesIfQueryLimitReached) {
                Thread.Sleep(1000 * trys);
              }
              break;
            default:
              //case "OVER_QUERY_LIMIT":
              //case "REQUEST_DENIED":
              //case "INVALID_REQUEST":
              //case "ZERO_RESULTS":
              status = $"status was {status}";
              hsc = HttpStatusCode.BadRequest;
              return;
          }
          trys++;
        } while (trys < maxTriesIfQueryLimitReached);

        status = $"unable to geocode";
        hsc = HttpStatusCode.BadRequest;
        return;
      } catch (Exception ex) {
        status = $"unexpected error";
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex);
      } finally {
        dtLastRequest = DateTime.UtcNow;
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
