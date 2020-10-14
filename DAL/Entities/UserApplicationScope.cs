using System;

namespace OAuthServer.DAL.Entities
{
    public class UserApplicationScope
    {
        public UserApplication UserApplication { get; set; }

        public Guid UserApplicationId { get; set; }

        public Scope Scope { get; set; }

        public Guid ScopeId { get; set; }
    }
}