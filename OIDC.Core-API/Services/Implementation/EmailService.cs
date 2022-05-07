using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using OAuthServer.DAL.Entities;
using OAuthServer.Services.Interface;
using RazorEngineCore;

namespace OAuthServer.Services.Implementation;

public class EmailService : IEmailService
{
    public async Task SendToUserAsync(string slug, User user, Dictionary<string, string> data = null)
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

        using MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("OIDC.Core", "noreply@oidc.core"));
        message.To.Add(new MailboxAddress(user.FullName(), user.Email));
        message.Subject = "Test email subject";

        BodyBuilder builder = new BodyBuilder
        {
            TextBody = textResult,
            HtmlBody = htmlResult
        };

        message.Body = builder.ToMessageBody();

        using SmtpClient client = new SmtpClient();
        await client.ConnectAsync("127.0.0.1", 1025);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendToEmailAsync(string slug, string email)
    {
        throw new System.NotImplementedException();
    }
}