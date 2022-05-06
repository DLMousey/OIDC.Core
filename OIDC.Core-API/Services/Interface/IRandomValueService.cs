namespace OAuthServer.Services.Interface
{
    public interface IRandomValueService
    {
        string CryptoSafeRandomString(int length = 64);
    }
}