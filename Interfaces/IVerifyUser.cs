using NetCoreIntermediate.Models;

namespace NetCoreIntermediate.Interfaces
{
    public interface IVerifyUser
    {
       Task<string> VerifyEmailAddress(TemporaryUser user);

       void SendEmail(EmailNotification emailNotification);

       Task<string> CheckOTPForEmailVerification(string OTP,string email);
    }
}
