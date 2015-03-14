﻿// @file
// A lightweight library for OAuth2 authentication.
//
// @author
// Stian Hanger <pdnagilum@gmail.com>
//
// @url
// https://github.com/nagilum/OAuth2Csharp

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// Authorization, refresh, and validation of users through OAuth2.
    /// </summary>
    public class OAuth2Client
    {
        /// <summary>
        /// JSON Serializer for deserializing the responses.
        /// </summary>
        protected static readonly JsonSerializer jsonSerializer = new JsonSerializer();

        /// <summary>
        /// The client credentials issued by the OAuth provider.
        /// </summary>
        private readonly OAuth2ClientCredentials clientCredentials;

        /// <summary>
        /// Name of provider.
        /// </summary>
        private OAuth2Endpoint endpoint { get; private set; }

        /// <summary>
        /// URL for provider to redirect back to when auth is completed.
        /// </summary>
        private string redirectURL { get; private set; }

        /// <summary>
        /// Access token issued by provider.
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Expiration date of the issued access token.
        /// </summary>
        public DateTime AccessTokenExpiration { get; private set; }

        /// <summary>
        /// The type of token issued by the provider.
        /// </summary>
        public string TokenType { get; private set; }

        /// <summary>
        /// Reflects whether or not the user has been authorized.
        /// </summary>
        public bool IsAuthorized { get; private set; }

        /// <summary>
        /// Parsed user-info from serialized info from provider.
        /// </summary>
        public OAuth2UserInfo UserInfo { get; private set; }

        /// <summary>
        /// Serialized string of the user-info response from provider.
        /// </summary>
        public string UserInfoSerialized { get; private set; }

        /// <summary>
        /// General error message from provider.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Reason for error, from provider.
        /// </summary>
        public string ErrorReason { get; private set; }

        /// <summary>
        /// Description of error, from provider.
        /// </summary>
        public string ErrorDescription { get; private set; }

        private readonly Action<string> redirect;

        /// <summary>
        /// Initiate a new instance of the OAuth2 library.
        /// </summary>
        /// <param name="clientCredentials">Client Credentials issued by the provider.</param>
        /// <param name="endpoint">The provider information.</param>
        /// <param name="redirectURL">URL for provider to redirect back to when auth is completed.</param>
        /// <param name="accessToken">(optional) Access token to refresh.</param>
        public OAuth2Client(OAuth2ClientCredentials clientCredentials, OAuth2Endpoint endpoint, string redirectURL = null, string accessToken = null)
        {
            this.clientCredentials = clientCredentials;
            this.endpoint = endpoint;
            this.redirectURL = redirectURL;
            this.AccessToken = accessToken;
        }

        /// <summary>
        /// Redirect the user to the providers auth-page, or attempt to re-validate the stored access-token.
        /// </summary>
        public void Authenticate()
        {
            List<Tuple<string, string>> parameters;
            string url;

            if (!string.IsNullOrWhiteSpace(this.AccessToken))
            {
                parameters = new List<Tuple<string, string>> {
                new Tuple<string, string>("access_token", this.AccessToken),
                new Tuple<string, string>("client_id", this.clientCredentials.Id),
                new Tuple<string, string>("client_secret", this.clientCredentials.Secret),
                new Tuple<string, string>("redirect_uri", this.redirectURL)
            };

                url =
                    endpoint.RefreshTokenURL + "?" +
                    this.buildQueryString(parameters);

                var resp = this.makeWebRequest(url);
                var code = "";

                try
                {
                    code = jsonSerializer.Deserialize<OAuth2CodeResponse>(resp.Result.ToJsonReader()).Code;
                }
                catch (Exception ex)
                {
                    this.Error = "Unable to parse JSON response.";
                    this.ErrorDescription = ex.Message;
                }

                if (!string.IsNullOrWhiteSpace(code))
                    this.handleCodeResponse(code);

                if (this.IsAuthorized)
                    return;
            }

            parameters = new List<Tuple<string, string>> {
            new Tuple<string, string>("client_id", clientCredentials.Id),
            new Tuple<string, string>("display", "page"),
            new Tuple<string, string>("locale", "en"),
            new Tuple<string, string>("redirect_uri", this.redirectURL),
            new Tuple<string, string>("response_type", "code")
        };

            if (!string.IsNullOrWhiteSpace(endpoint.Scope))
                parameters.Add(
                    new Tuple<string, string>("scope", endpoint.Scope));

            url =
                endpoint.AuthURL + "?" +
                this.buildQueryString(parameters);

            redirect(url);
        }

        /// <summary>
        /// Check for OAuth2 code response and attempt to validate it.
        /// </summary>
        public void HandleResponse()
        {
            if (this.request.QueryString["code"] == null)
                return;

            var code = this.request.QueryString["code"];
            var error = this.request.QueryString["error"];

            if (!string.IsNullOrWhiteSpace(code))
            {
                this.handleCodeResponse(code);
            }
            else if (!string.IsNullOrWhiteSpace(error))
            {
                this.Error = error;
                this.ErrorReason = this.request.QueryString["error_reason"];
                this.ErrorDescription = this.request.QueryString["error_description"];
            }
        }

        /// <summary>
        /// Validate a user by checking the 'code' variable against the provider.
        /// </summary>
        /// <param name="code">Code to validate.</param>
        private void handleCodeResponse(string code)
        {
            var parameters = new List<Tuple<string, string>> {
                new Tuple<string, string>("client_id", this.clientCredentials.Id),
                new Tuple<string, string>("redirect_uri", this.redirectURL),
                new Tuple<string, string>("client_secret", this.clientCredentials.Secret),
                new Tuple<string, string>("code", code)
            };

            var url =
                endpoint.AccessTokenURL + "?" +
                this.buildQueryString(parameters);

            var resp = this.makeWebRequest(url);

            this.analyzeAccessTokenResponse(resp);

            if (string.IsNullOrWhiteSpace(this.AccessToken) &&
                !this.IsAuthorized)
                return;

            parameters = new List<Tuple<string, string>> {
                new Tuple<string, string>("access_token", this.AccessToken)
            };

            url =
                endpoint.UserInfoURL + "?" +
                this.buildQueryString(parameters);

            resp = this.makeWebRequest(url);
            this.analyzeUserInfoResponse(resp);
        }

        /// <summary>
        /// Attempt to analyze access-token response, either in string or JSON format.
        /// </summary>
        /// <param name="resp">Strong or JSON response.</param>
        private void analyzeAccessTokenResponse(string resp)
        {
            if (resp == null)
                return;

            this.AccessToken = null;
            this.AccessTokenExpiration = DateTime.MinValue;

            if (resp.StartsWith("{") &&
                resp.EndsWith("}"))
            {
                try
                {
                    var cr = new JsonSerializer().Deserialize<OAuth2CodeResponse>(resp.ToJsonReader());

                    if (!string.IsNullOrWhiteSpace(cr.Access_Token))
                        this.AccessToken = cr.Access_Token;

                    if (cr.Expires_In > 0)
                        this.AccessTokenExpiration = DateTime.Now.AddSeconds(cr.Expires_In);

                    if (!string.IsNullOrWhiteSpace(cr.Token_Type))
                        this.TokenType = cr.Token_Type;
                }
                catch (Exception ex)
                {
                    this.Error = "Unable to parse JSON response.";
                    this.ErrorDescription = ex.Message;
                }
            }
            else
            {
                foreach (var entry in resp.Split('&'))
                {
                    if (entry.IndexOf('=') == -1)
                        continue;

                    var key = entry.Substring(0, entry.IndexOf('='));
                    var val = entry.Substring(entry.IndexOf('=') + 1);

                    switch (key)
                    {
                        case "access_token":
                            this.AccessToken = val;
                            break;

                        case "expires":
                        case "expires_in":
                            int exp;
                            if (int.TryParse(val, out exp))
                                this.AccessTokenExpiration = DateTime.Now.AddSeconds(exp);

                            break;

                        case "token_type":
                            this.TokenType = val;
                            break;
                    }
                }
            }

            this.IsAuthorized = (!string.IsNullOrWhiteSpace(this.AccessToken) &&
                                 this.AccessTokenExpiration > DateTime.Now);
        }

        /// <summary>
        /// Attempt to analyze the user-info JSON object from provider.
        /// </summary>
        /// <param name="resp">Serialized JSON object.</param>
        private void analyzeUserInfoResponse(string resp)
        {
            if (resp == null)
                return;

            this.UserInfoSerialized = resp;

            try
            {
                this.UserInfo = jsonSerializer.Deserialize<OAuth2UserInfo>(resp.ToJsonReader());
            }
            catch (Exception ex)
            {
                this.Error = "Unable to parse JSON response.";
                this.ErrorDescription = ex.Message;
            }
        }

        /// <summary>
        /// Compiles a list of parameters into a working query-string.
        /// </summary>
        /// <param name="parameters">Parametrs to compile.</param>
        /// <returns>Compilled query-string.</returns>
        private string buildQueryString(IEnumerable<Tuple<string, string>> parameters)
        {
            return string.Join("&", parameters.Where(param => !string.IsNullOrWhiteSpace(param.Item2)).Select(param => param.Item1 + "=" + param.Item2));
        }

        /// <summary>
        /// Perform a HTTP web request to a given URL.
        /// </summary>
        /// <param name="url">URL to request.</param>
        /// <returns>String of response.</returns>
        private async Task<string> makeWebRequest(string url)
        {
            var req = new HttpClient();
            return await req.GetStringAsync(url);
        }

        /// <summary>
        /// Object for json parsed code-response from provider.
        /// </summary>
        private class OAuth2CodeResponse
        {
            /// <summary>
            /// Code from provider.
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// Token issued by the provider.
            /// </summary>
            public string Access_Token { get; set; }

            /// <summary>
            /// Amount of second til token expires.
            /// </summary>
            public int Expires_In { get; set; }

            /// <summary>
            /// The type of token issued by the provider.
            /// </summary>
            public string Token_Type { get; set; }
        }
    }
}