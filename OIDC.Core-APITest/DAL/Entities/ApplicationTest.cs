using System;
using OAuthServer.DAL.Entities;
using OAuthServer.DAL.ViewModels.Entities;
using OAuthServer.Services.Implementation;
using OAuthServer.Services.Interface;
using Xunit;

namespace OIDC.Core_APITest.DAL.Entities;

public class ApplicationTest
{
    [Fact]
    public void NewInstanceHasGuid()
    {
        Application application = new Application();
        Assert.False(application.Id.Equals(Guid.Empty));
    }

    [Fact]
    public void ToViewModelHasCorrectData()
    {
        IRandomValueService rvs = new RandomValueService();

        Application application = new Application
        {
            Name = "Test Application",
            Description = "Test Application Description",
            HomepageUrl = "https://example.com",
            RedirectUrl = "https://example.com/oauth",
            ClientId = Guid.Empty,
            FirstParty = true
        };

        ApplicationViewModel avm = application.ToViewModel();
        
        Assert.Equal(application.Id, avm.Id);
        Assert.Equal(application.Name, avm.Name);
        Assert.Equal(application.Description, avm.Description);
        Assert.Equal(application.FirstParty, avm.FirstParty);
        Assert.Equal(application.HomepageUrl, avm.HomepageUrl);
        Assert.Equal(application.RedirectUrl, avm.RedirectUrl);
        Assert.Equal(application.ClientId, avm.ClientId);
    }
}