using Azure;
using Microsoft.AspNetCore.Mvc;
using NetCoreIntermediate.DbContextService;
using NetCoreIntermediate.Interfaces;
using NetCoreIntermediate.Models;
using NetCoreIntermediate.Services;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;



namespace NetCoreIntermediate.Repositories
{
    public class VerifyEmailAddressRepository : IVerifyUser
    {
        private readonly IBaseCrudService<AdminInfo,string> BaseCrudService;
        private readonly IBaseCrudService<OtpInfos, string> BaseOtpVerifySerivce;
        private readonly IBaseCrudService<TemporaryUser,string> BaseTemporaryUserService;
        private readonly IBaseCrudService<SignUpUser,string> BaseSignUpUserService;
        private readonly IConfiguration Configuration;
        private readonly JwtAuthenticationService GenerateJwt;
       

        public VerifyEmailAddressRepository(IBaseCrudService<AdminInfo, string> baseCrudService, IConfiguration configuration, JwtAuthenticationService jwtToken, IBaseCrudService<OtpInfos,string> baseOtpService, IBaseCrudService<TemporaryUser,string> temporaryUser, IBaseCrudService<SignUpUser,string> signUpUser)
        {
            Configuration = configuration;
            BaseCrudService = baseCrudService;
            GenerateJwt = jwtToken;
            BaseOtpVerifySerivce = baseOtpService;
            BaseTemporaryUserService = temporaryUser;
            BaseSignUpUserService = signUpUser;
        }

        public async Task<string> VerifyEmailAddress(TemporaryUser user)
        {
           
            if (user.Email != null)
            {
                int otpLength = 6;
                var OTP = OtpGenerator.GenerateUniqueDigitOTP(otpLength);

                var emailNotification = new EmailNotification
                {
                    Email = user.Email,
                    Subject = "Verify Your Email with Code",
                    Message = "Please Enter this code to verify your email and finish creating your account/n" +
                    $"<h1>{OTP}</h1>",
                    IsAccountCreated = true,
                };
                   
                    try
                    {
                        var otpmodel = new OtpInfos { email = user.Email, otp=OTP };
                        var otpResponse = await BaseOtpVerifySerivce.AddNewData(otpmodel);
                        await BaseTemporaryUserService.AddNewData(user);
                        if(!otpResponse.isSuccess)
                        {
                            throw new Exception($"Failed to send email: {otpResponse.message}");
                        }
                        try
                        {
                            SendEmail(emailNotification);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to send email: {ex.Message}");
                        }
                        return "Successfully sent the OTP to your email";

                    }
                    catch(Exception err)
                    {
                        throw new Exception($"Failed to send email: {err.Message}");
                    }

            }
           throw new Exception($"Email address in null");
        }

        public void SendEmail(EmailNotification data)
        {

            bool isAccountCreationSuccessfull = data.IsAccountCreated;
            if (isAccountCreationSuccessfull && data != null)
            {
                string? email = data?.Email;
                string? message = data?.Message;
                string? subject = data?.Subject;
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(subject))
                {
                    try
                    {
                        string smtpServer = "smtp.gmail.com";
                        int smtpPort = 587; // TLS port
                        string fromEmail = "sanasyed1998786@gmail.com";
                        string fromPassword = "bfwk heic yndq bzru";

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(fromEmail),
                            Subject = subject,
                            Body = message,
                            IsBodyHtml = true
                        };
                        mailMessage.To.Add(email);

                        using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                        {
                            smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                            smtpClient.EnableSsl = true; // Required for Gmail and most SMTP servers
                            smtpClient.Send(mailMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to send the Email {ex.Message}");
                    }
                }

            }

        }
        public async Task<string> CheckOTPForEmailVerification(string otp,  string emailAddress)
        {
            var OTP = await BaseOtpVerifySerivce.GetTheData(emailAddress);
            if(OTP.otp != otp)
            {
                throw new Exception("OTP entered is wrong, Try again");
            }

            var result = await BaseCrudService.GetTheData(emailAddress);
            var temporaryData = await BaseTemporaryUserService.GetTheData(emailAddress);
            
            var signUpUserData = new SignUpUser
            {
                Email = emailAddress,
                Name = temporaryData.Name,
                Password = temporaryData.Password,
                Role = result == null ? "User" : "Admin",
            };
            await BaseSignUpUserService.AddNewData(signUpUserData);
            await BaseTemporaryUserService.DeleteTheData(emailAddress);
            if (result == null)
            {
                return GenerateJwt.GenerateToken("user", emailAddress);
            }


            return GenerateJwt.GenerateToken("Admin", emailAddress);
        }

 
    }
}
