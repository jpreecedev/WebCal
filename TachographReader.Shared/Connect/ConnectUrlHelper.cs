namespace TachographReader.Shared.Connect
{
    public static class ConnectUrlHelper
    {
#if DEBUG
        public const string ServiceUrl = "http://webcal-connect.local/ConnectService.svc";
#else
        public const string ServiceUrl = "https://www.webcalconnect.com/service/ConnectService.svc";
#endif
    }
}