using System;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace OAuthServer.DAL.Entities
{
    public class AuthorisationCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid ApplicationId { get; set; }

        public Application Application { get; set; }
    }
}