using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;

namespace OAuthServer.Services.Interface;

public interface IEmailService
{
    Task SendToUserAsync(string slug, User user, Dictionary<string, string> data = null);

    // Task SendToEmailAsync(string slug, string email, Dictionary<string, string> data = null);
}