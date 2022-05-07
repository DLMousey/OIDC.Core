using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Emails;

namespace OAuthServer.Services.Interface;

public interface IEmailService
{
    Task SendToUserAsync(EmailViewModel viewModel, User user);

    Task SendToEmailAsync(EmailViewModel viewModel);
}