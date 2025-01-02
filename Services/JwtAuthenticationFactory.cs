using AuthenticationService.Models;
using NetCoreIntermediate.Services;

namespace AuthenticationService.Services
{
    public class JwtAuthenticationFactory
    {
        private static JwtAuthenticationService _instance;
        private static readonly object LockObject = new object();
        private readonly JwtSettings _settings;

        public static JwtAuthenticationService GetInstance(string secretKey, string issuer, string audience)
        {
            if (_instance == null)
            {
                lock (LockObject)
                {
                    if (_instance == null)
                        
                    {
                        _instance = new JwtAuthenticationService(secretKey, issuer, audience);
                    }
                }
            }

            return _instance;
        }
    }

}
