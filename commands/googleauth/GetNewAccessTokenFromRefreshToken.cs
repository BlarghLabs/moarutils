using MoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading;

namespace MoarUtils.Utils.GoogleAuth {
  public class GetNewAccessTokenFromRefreshToken {
    public class request {
      public string clientId { get; set; }
      public string clientSecret { get; set; }
      public string refreshToken { get; set; }
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
    /// if successed, should save accesstoken and expire and use until expires
    /// if fails shoudl consider clearing refresh token
    /// </summary>
    public static void Execute(
      request m,
      out response r,
      out HttpStatusCode hsc,
      out string status,
      CancellationToken? ct = null
    ) {
      r = new response { };
      status = "";
      hsc = HttpStatusCode.BadRequest;
      try {
        var client = new RestClient {
          BaseUrl = new Uri("https://accounts.google.com"),
        };
        //https://developers.google.com/accounts/docs/OAuth2WebServer
        //#Using a Refresh Token
        var request = new RestRequest {
          Resource = "o/oauth2/token",
          Method = Method.POST,
        };
        request.AddParameter("client_id", m.clientId);
        request.AddParameter("client_secret", m.clientSecret);
        request.AddParameter("refresh_token", m.refreshToken);
        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("Content-Type", "application/x-www-form-urlencoded");
        var response = client.Execute(request);
        //valid response: { "access_token":"1/XXX", "expires_in":3920, "token_type":"Bearer",}
        if (response.StatusCode != HttpStatusCode.OK) {
          status = "Unable to get new access token: " + response.StatusCode.ToString() + "|" + response.Content;
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        var content = response.Content;
        //LogIt.D(content);
        dynamic json = JObject.Parse(content);
        r = new response {
          access_token = json.access_token,
          expires_in = json.expires_in,
          token_type = json.token_type,
          refresh_token = json.refresh_token
        };

        //if (string.IsNullOrEmpty(tr.access_token)) {
        //  //should I clear out refesh token?
        //  LogIt.E("access token was null");
        //  //} else {
        //  //  icalgenie.lib.Db.Credential.AddOrUpdateUserCredentials(new credential_element { name = CredentialElementName.GoogleAccessToken.ToString(), value = tr.access_token }, u, SourceType.GmailAccount);
        //  //}
        //}

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        status = "unepxected error"; //was: errorMsg = ex.Message;
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex);
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          m,
          //r,
        }));
      }
    }
  }
}
