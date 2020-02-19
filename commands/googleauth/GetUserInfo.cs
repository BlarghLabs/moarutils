using MoarUtils.commands.logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Threading;

namespace MoarUtils.Utils.GoogleAuth {
  public class GetUserInfo {
    public class request {
      public string clientId { get; set; }
      public string clientSecret { get; set; }
      public string refreshToken { get; set; }
    }

    public class response {
      public string accessToken { get; set; }
      public string email { get; set; }

    }

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
      var content = "";
      try {
        if (string.IsNullOrEmpty(m.refreshToken)) {
          status = "refresh token is required";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        GetNewAccessTokenFromRefreshToken.Execute(
          hsc: out hsc,
          status: out status,
          m: new GetNewAccessTokenFromRefreshToken.request {
            clientId = m.clientId,
            refreshToken = m.refreshToken,
            clientSecret = m.clientSecret,
          },
          r: out GetNewAccessTokenFromRefreshToken.response r1,
          ct: ct
        );
        if (hsc != HttpStatusCode.OK) {
          status = "unable to GetNewAccessTokenFromRefreshToken";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        r.accessToken = r1.access_token;
        var client = new RestClient("https://www.googleapis.com/");
        var request = new RestRequest("oauth2/v2/userinfo", Method.GET);
        request.AddHeader("Authorization", "Bearer " + r.accessToken); //Authorization: Bearer XXX
        var response = client.Execute(request);
        content = response.Content;  //{ "id": "XXX", "email": "foo@bar.com", "verified_email": true}

        dynamic json = JsonConvert.DeserializeObject(content);
        r.email = json.email.Value;

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
          content,
          //r,
        }, Formatting.Indented));
      }
    }
  }
}

