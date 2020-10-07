using System;

namespace OAuthServer.DAL.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid ApplicationId { get; set; }

        public Application Application { get; set; }

        public bool Revoked { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; }

        public DateTime? LastUsed { get; set; }
    }
}