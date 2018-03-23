using System;

namespace LT.HttpClientHelper.TestConsole.Models
{
    /// <summary>
    /// Contract for user
    /// </summary>
    public class UserContract
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Person name
        /// </summary>
        public virtual string PersonName { get; set; }

        /// <summary>
        /// Person surname
        /// </summary>
        public virtual string PersonSurname { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Is User Enabled
        /// </summary>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// Is User Locked
        /// </summary>
        public virtual bool IsLocked { get; set; }

        /// <summary>
        /// Registration date
        /// </summary>
        public virtual DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Last access date
        /// </summary>
        public virtual DateTime? LastAccessDate { get; set; }

        /// <summary>
        /// Is SuperAdmin  user
        /// </summary>
        public virtual bool IsSuperAdmin { get; set; }

        /// <summary>
        /// Photo
        /// </summary>
        public virtual byte[] PhotoBinary { get; set; }
    }
}
