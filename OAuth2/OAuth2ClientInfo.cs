using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// Represents the client information issued by the OAuth provider.
    /// </summary>
    public class OAuth2ClientInfo
    {
        /// <summary>
        /// Gets the Client Secret issued by the provider.
        /// </summary>
        public string Secret { get; private set; }

        /// <summary>
        /// Gets the Client Id issued by the provider.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="OAuth2ClientInfo"/> class with the given details.
        /// </summary>
        /// <param name="is">The Client Id issued by the provider.</param>
        /// <param name="secret">The Client Secret issued by the provider.</param>
        public OAuth2ClientInfo(string id, string secret)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ClientID is required!", "id");

            if (string.IsNullOrWhiteSpace(secret))
                throw new ArgumentException("ClientSecret is required!", "secret");

            Id = id;
            Secret = secret;
        }
    }
}