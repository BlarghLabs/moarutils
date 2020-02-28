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
      public string picture { get; set; }
      public string id { get; set; }
      public bool verifiedEmail { get; set; }
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

        if (response.StatusCode != HttpStatusCode.OK) {
          status = $"StatusCode was {response.StatusCode}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (response.ErrorException != null) {
          status = $"response had error exception: {response.ErrorException.Message}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (string.IsNullOrWhiteSpace(response.Content)) {
          status = $"content was empty";
          hsc = HttpStatusCode.BadRequest;
          return;
        }

        content = response.Content;
        dynamic json = JsonConvert.DeserializeObject(content);

        #region cheat sheet
        //{
        //  {
        //    "id": "123456789012345678901",
        //    "email": "foo@bar.baz",
        //    "verified_email": true,
        //    "picture": "https://lh3.googleusercontent.com/a-/1111111111111111111111111111111111111111111111"
        // }
        //}
        #endregion

        r.email = json.email == null
          ? null
          : json.email.Value
        ;
        r.picture = json.picture == null
          ? null
          : json.picture.Value
        ;
        r.id = json.id == null
          ? null
          : json.id.Value
        ;
        r.verifiedEmail = json.verified_email == null
          ? false
          : json.verified_email.Value
        ;

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
          //m, //logging
          //content, //logging
          //r,
          r.email,
          r.picture,
          r.verifiedEmail,
        }, Formatting.Indented));
      }
    }
  }
}

