using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// Endpoint to use for validation.
    /// </summary>
    public class OAuth2Endpoint
    {
        /// <summary>
        /// Name of provider.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// URL for user-redirection to provider auth-page.
        /// </summary>
        public string AuthURL { get; set; }

        /// <summary>
        /// URL for access token validation.
        /// </summary>
        public string AccessTokenURL { get; set; }

        /// <summary>
        /// URL for access token refresh.
        /// </summary>
        public string RefreshTokenURL { get; set; }

        /// <summary>
        /// URL for user infomation gathering.
        /// </summary>
        public string UserInfoURL { get; set; }

        /// <summary>
        /// Provider-scope, if any.
        /// </summary>
        public string Scope { get; set; }
    }
}