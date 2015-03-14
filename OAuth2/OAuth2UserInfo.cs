using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2
{
    /// <summary>
    /// Object to store providers user-info.
    /// </summary>
    public class OAuth2UserInfo
    {
        /// <summary>
        /// ID issued by provider.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// E-mail for user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name of user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name of user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gender of user.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Locale of user.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Time-zone of user.
        /// </summary>
        public int TimeZone { get; set; }

        /// <summary>
        /// Username of user.
        /// </summary>
        public string Username { get; set; }
    }
}