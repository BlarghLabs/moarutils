using MoarUtils.Model;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Web;

namespace MoarUtils.Utils.Gis.Geocode {
  public static class ViaGoogle {
    private const double m_dThrottleSeconds = .9; //1.725; //0.1; //100ms //Convert.ToDouble(1.725);
    private static DateTime dtLastRequest = DateTime.Now;
    private static Mutex mLastRequest = new Mutex();
    
    private static string GetUrlSecondPart(string location) {
      string locationUrl = "";
      if (!String.IsNullOrEmpty(location)) {
        locationUrl = "address=" + Uri.EscapeDataString(Strings.CondenseWhiteSpace(HttpUtility.UrlEncode(location.Replace("+", " "))));
      }
      return "maps/api/geocode/json?" + locationUrl + "&sensor=false";
    }

    public static Coordinate Execute(string address, bool useRateLimit, WebProxy wp = null, int maxTriesIfQueryLimitReached = 1, bool throwOnUnableToGeocode = true) {
      lock (mLastRequest) {
        //Force delay of 1.725 seconds between requests: re: http://groups.google.com/group/Google-Maps-API/browse_thread/thread/906e871bcb8c15fd
        TimeSpan tsDuration;
        bool bRequiredWaitTimeHasElapsed;
        do {
          tsDuration = DateTime.Now - dtLastRequest;
          bRequiredWaitTimeHasElapsed = (tsDuration.TotalSeconds > m_dThrottleSeconds);
          if (!bRequiredWaitTimeHasElapsed) {
            int iMillisecondsToSleep = Convert.ToInt32((m_dThrottleSeconds - tsDuration.TotalSeconds) * Convert.ToDouble(1000));
            Thread.Sleep(iMillisecondsToSleep);
          }
        } while (!bRequiredWaitTimeHasElapsed);
        return Execute(address, wp, maxTriesIfQueryLimitReached, throwOnUnableToGeocode);
      }
    }

    public static Coordinate Execute(string address, WebProxy wp = null, int maxTriesIfQueryLimitReached = 1, bool throwOnUnableToGeocode = true) {
      try {
        int trys = 1;
        string lat = "", lng = "", location_type = "", status = "";
        do {
          //var client = new RestClient("http://api.externalip.net/");
          //var request = new RestRequest("ip", Method.GET);
          var client = new RestClient("http://maps.googleapis.com/");
          var request = new RestRequest(GetUrlSecondPart(address), Method.GET);
          request.RequestFormat = DataFormat.Json;
          if (wp != null) {
            client.Proxy = wp;
          }
          var response = client.Execute(request);
          if (response.ErrorException != null) {
            throw response.ErrorException;
          }
          var content = response.Content;
          dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
          status = json.status.Value;
          switch (status) {
            case "OK":
              var results = json.results;
#if DEBUG
                Console.WriteLine(results);
#endif
              lat = json.results[0].geometry.location.lat;
              lng = json.results[0].geometry.location.lng;
              location_type = json.results[0].geometry.location_type;
              lat = (lat == null) ? "" : lat;
              lng = (lng == null) ? "" : lng;
              location_type = (location_type == null) ? "" : location_type;
              return new Coordinate {
                lat = Convert.ToDecimal(lat),
                lng = Convert.ToDecimal(lng),
                precision = location_type
              };
            case "UNKNOWN_ERROR":
              if (trys < maxTriesIfQueryLimitReached) {
                Thread.Sleep(1000 * trys);
              }
              break;
            default:
            case "OVER_QUERY_LIMIT":
            case "REQUEST_DENIED":
            case "INVALID_REQUEST":
            case "ZERO_RESULTS":
              throw new Exception(status);
          }
          trys++;
        } while (trys < maxTriesIfQueryLimitReached);
        throw new Exception("unable to geocode:" + status);
      } catch (Exception ex) {
        LogIt.W(ex.Message + "|" + address);
        if (throwOnUnableToGeocode) {
          throw ex;
        } else {
          return new Coordinate {
            lat = 0,
            lng = 0
            //TODO: denote error caught...
          };
        }
      } finally {
        dtLastRequest = DateTime.Now;
      }
    }


  }
}
