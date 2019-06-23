using MoarUtils.commands.logging;
using MoarUtils.Model;
using RestSharp;
using System;


namespace MoarUtils.Utils.GoogleAuth {
  public class GetUserInfo {
    public static bool Execute(string clientId, string clientSecret, string rt, out string at, out string email, out string errorMsg) {
      email = "";
      at = "";
      errorMsg = "";
      try {
        if (string.IsNullOrEmpty(rt)) {
          errorMsg = "refresh token is required";
          LogIt.D(errorMsg);
          return false;
        } else {
          //get and refesh access_token
          OAuth2TokenResponse tr;
          if (!GetNewAccessTokenFromRefreshToken.Execute(clientId, clientSecret, rt, out tr, out errorMsg)) {
            return false;
          } else {
            at = tr.access_token;
            var client = new RestClient("https://www.googleapis.com/");
            var request = new RestRequest("oauth2/v2/userinfo", Method.GET);
            request.AddHeader("Authorization", "Bearer " + at); //Authorization: Bearer XXX
            var response = client.Execute(request);
            var content = response.Content;  //{ "id": "XXX", "email": "foo@bar.com", "verified_email": true}
            LogIt.I(content);

            dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
            email = json.email;

            return true;
          }
        }
      } catch (Exception ex) {
        errorMsg = ex.Message;
        LogIt.E(ex);
        return false;
      }
    }
  }
}
