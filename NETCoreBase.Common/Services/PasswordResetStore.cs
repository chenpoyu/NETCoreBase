using System;
using System.Collections.Concurrent;
using System.Linq;
using NETCoreBase.Common.Interfaces;

namespace NETCoreBase.Common.Services
{
    /// <summary>
    /// Singleton in-memory store for password-reset tokens.
    /// </summary>
    public class PasswordResetStore : IPasswordResetStore
    {
        private sealed record Entry(string Username, DateTime Expiry);

        private readonly ConcurrentDictionary<string, Entry> _store = new();

        /// <inheritdoc/>
        public void Store(string token, string username)
        {
            // Purge stale entries
            var now = DateTime.UtcNow;
            foreach (var key in _store.Keys.ToList())
            {
                if (_store.TryGetValue(key, out var e) && e.Expiry < now)
                    _store.TryRemove(key, out _);
            }
            _store[token] = new Entry(username, now.AddMinutes(15));
        }

        /// <inheritdoc/>
        public string Consume(string token)
        {
            if (!_store.TryRemove(token, out var entry))
                return null;

            return entry.Expiry >= DateTime.UtcNow ? entry.Username : null;
        }
    }
}
