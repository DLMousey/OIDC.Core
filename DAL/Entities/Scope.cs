using System;

namespace OAuthServer.DAL.Entities
{
    public class Scope
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Description { get; set; }
    }
}