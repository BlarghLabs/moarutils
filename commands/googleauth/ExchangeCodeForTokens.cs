using MoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;

namespace MoarUtils.Utils.GoogleAuth {
  public class ExchangeCodeForTokens {
    public class request {
      public string code { get; set; }
      public string clientId { get; set; }
      public string clientSecret { get; set; }
      public string redirectUrl { get; set; }
    }

    public class response {
      //{
      //  "access_token" : "XXX",
      //  "token_type" : "Bearer",
      //  "expires_in" : 3600,
      //  "refresh_token" : "1/XXX"
      //}

      public string access_token { get; set; }
      public string token_type { get; set; }
      public string refresh_token { get; set; }
      public int expires_in { get; set; }
    }

    /// <summary>
    /// On success save refresh and authtoken
    /// </summary>
    public static void Execute(
      request m,
      out response r,
      out string status,
      out HttpStatusCode hsc
    ) {
      r = new response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        //POST /o/oauth2/token HTTP/1.1
        //Host: accounts.google.com
        //Content-length: 250
        //content-type: application/x-www-form-urlencoded
        //user-agent: google-oauth-playground

        //code=XXX&redirect_uri=https%3A%2F%2Fdevelopers.google.com%2Foauthplayground&client_id=XXX.apps.googleusercontent.com&scope=&client_secret=************&grant_type=authorization_code

        var request = new RestRequest("o/oauth2/token", Method.POST);
        request.AddParameter("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("code", m.code);
        request.AddParameter("redirect_uri", m.redirectUrl);
        request.AddParameter("client_id", m.clientId);
        request.AddParameter("scope", "");
        request.AddParameter("client_secret", m.clientSecret);
        request.AddParameter("grant_type", "authorization_code");

        var client = (new RestClient("https://accounts.google.com/"));
        var response = client.Execute(request);
        var content = response.Content;

        //valid response: 
        //{
        //  "access_token" : "XXX",
        //  "token_type" : "Bearer",
        //  "expires_in" : 3600,
        //  "refresh_token" : "XXX"
        //}

        if (response.StatusCode != HttpStatusCode.OK) {
          status = response.StatusCode.ToString() + "|" + response.Content;
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        //LogIt.D(content);
        dynamic json = JObject.Parse(content);
        r = new response {
          access_token = json.access_token,
          expires_in = json.expires_in,
          refresh_token = json.refresh_token,
          token_type = json.token_type
        };
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexpected error";
      } finally {
        JsonConvert.SerializeObject(new {
          hsc,
          status,
          m
        }, Formatting.Indented);
      }
    }
  }
}
