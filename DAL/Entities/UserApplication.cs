using System;
using System.Collections;
using System.Collections.Generic;

namespace OAuthServer.DAL.Entities
{
    public class UserApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid ApplicationId { get; set; }

        public Application Application { get; set; }

        public ICollection<UserApplicationScope> Scopes { get; set; }
    }
}