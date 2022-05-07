using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Emails;
using OAuthServer.Services.Interface;
using RazorEngineCore;

namespace OAuthServer.Services.Implementation;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfigurationSection _emailConfig;

    private readonly string _fromName;
    private readonly string _fromAddress;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _emailConfig = configuration.GetSection("email");
        _fromName = _emailConfig.GetValue<string>("from:name", "OIDC.Core");
        _fromAddress = _emailConfig.GetValue<string>("from:address", "noreply@oidc.core");
    }
    
    public async Task SendToUserAsync(EmailViewModel viewModel, User user)
    {
        try
        {
            Tuple<string, string> templates = await BuildTemplesAsync(viewModel.Slug, viewModel.Data);
            MimeMessage message = BuildMessage(
                new Tuple<string, string>(_fromName, _fromAddress),
                new Tuple<string, string>(user.FullName(), user.Email),
                templates,
                viewModel.Subject
            );

            SendAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to send email to user for config: {ViewModel}", JsonConvert.SerializeObject(viewModel));
        }
    }

    public async Task SendToEmailAsync(EmailViewModel viewModel)
    {
        try
        {
            Tuple<string, string> templates = await BuildTemplesAsync(viewModel.Slug, viewModel.Data);
            
            MimeMessage message = BuildMessage(
                new Tuple<string, string>(_fromName, _fromAddress),
                new Tuple<string, string>(viewModel.ToName, viewModel.ToAddress),
                templates,
                viewModel.Subject
            );

            SendAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to send email to email address for config: {ViewModel}", JsonConvert.SerializeObject(viewModel));
        }
    }

    private async Task<Tuple<string, string>> BuildTemplesAsync(string slug, Dictionary<string, string> data)
    {
        string workingDirectory = Environment.CurrentDirectory;
        string templatePath = Path.Join(workingDirectory, "Resources", "Email", slug);

        string htmlPart = await File.ReadAllTextAsync(Path.Join(templatePath, "template-html.cshtml"));
        string textPart = await File.ReadAllTextAsync(Path.Join(templatePath, "template-text.cshtml"));

        IRazorEngine razorEngine = new RazorEngine();
        
        IRazorEngineCompiledTemplate<RazorEngineTemplateBase<Dictionary<string, string>>> htmlTemplate = 
            await razorEngine.CompileAsync<RazorEngineTemplateBase<Dictionary<string, string>>>(htmlPart);
        IRazorEngineCompiledTemplate<RazorEngineTemplateBase<Dictionary<string, string>>> textTemplate = 
            await razorEngine.CompileAsync<RazorEngineTemplateBase<Dictionary<string, string>>>(textPart);

        string htmlResult = await htmlTemplate.RunAsync(instance => instance.Model = data);
        string textResult = await textTemplate.RunAsync(instance => instance.Model = data);

        return new Tuple<string, string>(htmlResult, textResult);
    }

    private MimeMessage BuildMessage(Tuple<string, string> from, Tuple<string, string> to, Tuple<string, string> templates, string subject)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(from.Item1, from.Item2));
        message.To.Add(new MailboxAddress(to.Item1, to.Item2));
        message.Subject = subject;

        BodyBuilder builder = new BodyBuilder
        {
            HtmlBody = templates.Item1,
            TextBody = templates.Item2
        };

        message.Body = builder.ToMessageBody();

        return message;
    }

    private async void SendAsync(MimeMessage message)
    {
        SmtpClient client = new SmtpClient();
        await client.ConnectAsync("127.0.0.1", 1025);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        client.Dispose();
        message.Dispose();
        
        _logger.LogInformation("Email dispatched");
    }
}