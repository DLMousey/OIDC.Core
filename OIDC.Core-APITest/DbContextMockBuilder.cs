using Microsoft.EntityFrameworkCore;
using Moq;
using OAuthServer.DAL;

namespace OIDC.Core_APITest;

public static class DbContextMockBuilder
{
    public static Mock<AppDbContext> BuildDbContextMock()
    {
        DbContextOptionsBuilder<AppDbContext> builder = new DbContextOptionsBuilder<AppDbContext>();
        return new Mock<AppDbContext>(builder.Options);
    }
}