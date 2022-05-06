using System;

namespace OAuthServer.DAL.Entities;

public class AccountEvent
{
    public Guid Guid { get; set; } = Guid.NewGuid();

    public string Type { get; set; }

    public string UserAgent { get; set; }

    public string IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }
}