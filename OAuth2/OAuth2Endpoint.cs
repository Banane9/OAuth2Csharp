using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// Represents an endpoint to use for validation.
    /// </summary>
    public class OAuth2Endpoint
    {
        /// <summary>
        /// Gets the name of provider.
        /// </summary>
        public string Provider { get; private set; }

        /// <summary>
        /// Gets the URL for user-redirection to provider auth-page.
        /// </summary>
        public string AuthURL { get; private set; }

        /// <summary>
        /// Gets the URL for access token validation.
        /// </summary>
        public string AccessTokenURL { get; private set; }

        /// <summary>
        /// Gets the URL for access token refresh.
        /// </summary>
        public string RefreshTokenURL { get; private set; }

        /// <summary>
        /// Gets the URL for user infomation gathering.
        /// </summary>
        public string UserInfoURL { get; private set; }

        /// <summary>
        /// Gets the Provider-scope.
        /// </summary>
        public string Scope { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="OAuth2Endpoint"/> class with the given information.
        /// </summary>
        /// <param name="provider">Name of the provider.</param>
        /// <param name="authURL">URL for user-redirection to provider auth-page.</param>
        /// <param name="accessTokenURL">URL for access token validation.</param>
        /// <param name="refreshTokenURL">URL for access token refresh.</param>
        /// <param name="userInfoURL">URL for user infomation gathering.</param>
        /// <param name="scope">Provider-scope, if any.</param>
        public OAuth2Endpoint(string provider, string authURL, string accessTokenURL, string refreshTokenURL, string userInfoURL, string scope = "")
        {
            Provider = provider;
            AuthURL = authURL;
            AccessTokenURL = accessTokenURL;
            RefreshTokenURL = refreshTokenURL;
            UserInfoURL = userInfoURL;
            Scope = scope;
        }
    }
}