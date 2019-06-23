using MoarUtils.commands.logging;
using MoarUtils.Model;
using RestSharp;
using System;

namespace MoarUtils.Utils.GoogleAuth {
  public class ExchangeCodeForTokens {
    /// <summary>
    /// On success save refresh and authtoken
    /// </summary>
    /// <param name="code"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    public static OAuth2TokenResponse Execute(string code, string clientId, string clientSecret, string redirectUrl) {
      try {
        //POST /o/oauth2/token HTTP/1.1
        //Host: accounts.google.com
        //Content-length: 250
        //content-type: application/x-www-form-urlencoded
        //user-agent: google-oauth-playground

        //code=XXX&redirect_uri=https%3A%2F%2Fdevelopers.google.com%2Foauthplayground&client_id=XXX.apps.googleusercontent.com&scope=&client_secret=************&grant_type=authorization_code

        var request = new RestRequest("o/oauth2/token", Method.POST);
        request.AddParameter("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("code", code);
        request.AddParameter("redirect_uri", redirectUrl);
        request.AddParameter("client_id", clientId);
        request.AddParameter("scope", "");
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("grant_type", "authorization_code");

        var response = (new RestClient("https://accounts.google.com/")).Execute(request);
        var content = response.Content;  

        //valid response: 
        //{
        //  "access_token" : "XXX",
        //  "token_type" : "Bearer",
        //  "expires_in" : 3600,
        //  "refresh_token" : "XXX"
        //}

        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
          LogIt.W(response.StatusCode.ToString() + "|" + response.Content);
          return new OAuth2TokenResponse { };
        } else {
          //LogIt.D(content);
          dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
          var tr = new OAuth2TokenResponse {
            access_token = json.access_token,
            expires_in = json.expires_in,
            refresh_token = json.refresh_token,
            token_type = json.token_type
          };
          return tr;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }
  }
}
