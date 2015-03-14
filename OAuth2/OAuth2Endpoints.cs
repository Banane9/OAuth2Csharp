using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// A set of pre-defined provider endpoints.
    /// </summary>
    public static class OAuth2Endpoints
    {
        public static readonly OAuth2Endpoint Facebook = new OAuth2Endpoint
        (
            provider: "Facebook",
            authURL: "https://graph.facebook.com/oauth/authorize",
            accessTokenURL: "https://graph.facebook.com/oauth/access_token",
            refreshTokenURL: "https://graph.facebook.com/oauth/client_code",
            userInfoURL: "https://graph.facebook.com/me"
        );
    }
}