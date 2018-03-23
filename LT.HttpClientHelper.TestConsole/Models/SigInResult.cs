using System;
using System.Collections.Generic;

namespace LT.HttpClientHelper.TestConsole.Models
{
    public class SignInResult
    {
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Person name
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// Person surname
        /// </summary>
        public string PersonSurname { get; set; }

        ///// <summary>
        ///// Flag for enable user
        ///// </summary>
        //public bool IsEnabled { get; set; }

        /// <summary>
        /// Last access date
        /// </summary>
        public DateTime? LastAccessDate { get; set; }

        ///// <summary>
        ///// Flag for locked user (ex. too much tentatives)
        ///// </summary>
        //public bool IsLocked { get; set; }

        ///// <summary>
        ///// Provider used for sign-in
        ///// </summary>
        //public string SignInProvider { get; set; }

        /// <summary>
        /// Flag for SuperAdmin User
        /// </summary>
        public virtual bool IsSuperAdmin { get; set; }

        /// <summary>
        /// List of Apps
        /// </summary>
        public IList<UserLicenses> Apps { get; set; }

        public SignInResult()
        {
            Apps = new List<UserLicenses>();
        }

        /// <summary>
        /// TOKEN: Access Token for OAuth
        /// </summary>
        public virtual string AccessToken { get; set; }

        /// <summary>
        /// TOKEN: Token Type fo OAuth (bearer)
        /// </summary>
        public virtual string TokenType { get; set; }

        /// <summary>
        /// TOKEN: Expiration datetime of access token
        /// </summary>
        public virtual DateTime ExpirationToken { get; set; }

        /// <summary>
        /// TOKEN: Refresh Token
        /// </summary>
        public virtual string RefreshToken { get; set; }

        /// <summary>
        /// TOKEN: OAuth api Uri
        /// </summary>
        public virtual string BaseApiUri { get; set; }

    }
}
