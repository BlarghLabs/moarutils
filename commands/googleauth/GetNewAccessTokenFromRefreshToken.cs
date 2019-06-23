using MoarUtils.commands.logging;
using MoarUtils.Model;
using RestSharp;
using System;

namespace MoarUtils.Utils.GoogleAuth {
  public class GetNewAccessTokenFromRefreshToken {
    /// <summary>
    /// if successed, should save accesstoken and expire and use until expires
    /// if fails shoudl consider clearing refresh token
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="refreshToken"></param>
    /// <param name="tr"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static bool Execute(string clientId, string clientSecret, string refreshToken, out OAuth2TokenResponse tr, out string errorMsg) {
      tr = null;
      errorMsg = null;
       
      try {
        //https://developers.google.com/accounts/docs/OAuth2WebServer
        //#Using a Refresh Token

        var request = new RestRequest("o/oauth2/token", Method.POST);
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("refresh_token", refreshToken);
        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("Content-Type", "application/x-www-form-urlencoded");

        var response = (new RestClient("https://accounts.google.com")).Execute(request);
        //valid response: { "access_token":"1/XXX", "expires_in":3920, "token_type":"Bearer",}
        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
          errorMsg = "Unable to get new access token: " + response.StatusCode.ToString() + "|" + response.Content;
          LogIt.W(errorMsg);
          return false;
        } else {
          var content = response.Content;
          //LogIt.D(content);

          dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
          tr = new OAuth2TokenResponse {
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
          return true;
        }
      } catch (Exception ex) {
        errorMsg = ex.Message;
        LogIt.E(ex);
        return false;
      }
    }


    /// <summary>
    /// if successed, should save accesstoken and expire and use until expires
    /// if fails shoudl consider clearing refresh token
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [Obsolete("Method is deprecated, please use other execute instead.")]
    public static OAuth2TokenResponse Execute(string clientId, string clientSecret, string refreshToken) {
      try {
        //https://developers.google.com/accounts/docs/OAuth2WebServer
        //#Using a Refresh Token

        var request = new RestRequest("o/oauth2/token", Method.POST);
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("refresh_token", refreshToken);
        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("Content-Type", "application/x-www-form-urlencoded");

        var response = (new RestClient("https://accounts.google.com")).Execute(request);
        //valid response: { "access_token":"1/XXX", "expires_in":3920, "token_type":"Bearer",}
        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
          LogIt.W("Unable to get new access token: " + response.StatusCode.ToString() + "|" + response.Content);
          return new OAuth2TokenResponse { };
        } else {
          var content = response.Content;
          //LogIt.D(content);

          dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
          var tr = new OAuth2TokenResponse {
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
          return tr;
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }
  }
}
