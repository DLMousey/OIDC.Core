using System;

namespace OAuthServer.DAL.Entities
{
    public class UserApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid ApplicationId { get; set; }

        public Application Application { get; set; }

        public Guid? AuthorisationCodeId { get; set; }

        public AuthorisationCode AuthorisationCode { get; set; }

        public Guid? AccessTokenId { get; set; }

        public AccessToken AccessToken { get; set; }

        public Guid? RefreshTokenId { get; set; }

        public RefreshToken RefreshToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}