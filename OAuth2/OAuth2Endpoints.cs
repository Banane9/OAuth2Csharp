using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// A list of pre-defined provider endpoints.
    /// </summary>
    public static class OAuth2Endpoints
    {
        public static readonly OAuth2Endpoint Facebook = new OAuth2Endpoint
        {
            Provider = "Facebook",
            AuthURL = "https://graph.facebook.com/oauth/authorize",
            AccessTokenURL = "https://graph.facebook.com/oauth/access_token",
            RefreshTokenURL = "https://graph.facebook.com/oauth/client_code",
            UserInfoURL = "https://graph.facebook.com/me"
        };
    }
}