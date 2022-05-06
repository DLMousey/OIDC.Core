using System;
using System.Collections.Generic;

namespace OAuthServer.DAL.Entities
{
    public class AccessToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; }

        public Guid ApplicationId { get; set; }

        public Application Application { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public bool Revoked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; }

        public DateTime? LastUsed { get; set; }

        public DateTime? RevokedAt { get; set; }
    }
}