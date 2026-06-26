using System;

namespace NETCoreBase.Common.Interfaces
{
    /// <summary>
    /// Thread-safe, in-memory store for password-reset tokens.
    /// </summary>
    public interface IPasswordResetStore
    {
        /// <summary>Stores a reset token for the given username with a 15-minute TTL.</summary>
        void Store(string token, string username);

        /// <summary>
        /// Validates the token. Returns the username if valid, null if not found or expired.
        /// Consuming the token removes it.
        /// </summary>
        string Consume(string token);
    }
}
