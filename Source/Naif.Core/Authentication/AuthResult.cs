using System;

namespace Naif.Core.Authentication
{
    public class AuthResult
    {
        public AuthResult()
        {
            AquiredAt = DateTime.UtcNow;
        }

        public string AccessToken { get; set; }

        public DateTime AquiredAt { get; set; }

        public DateTime ExpiresAt
        {
            get
            {
                // subtract a minute from the expiration force refresh as we approach expiration
                return AquiredAt + TimeSpan.FromSeconds(ExpiresIn - 60);
            }
        }

        public int ExpiresIn { get; set; }

        public bool IsExpired
        {
            get
            {
                return DateTime.UtcNow > ExpiresAt;
            }
        }

        public string RefreshToken { get; set; }
        
        public string TokenType { get; set; }

    }
}
