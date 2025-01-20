using Microsoft.AspNetCore.Mvc;
using NetCoreIntermediate.Interfaces;
using NetCoreIntermediate.Models;
using NetCoreIntermediate.Repositories;
using System.Security.Cryptography.Pkcs;
using static System.Net.WebRequestMethods;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreIntermediate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpUserController : ControllerBase
    {
        private readonly IBaseCrudService<TemporaryUser,string> BaseCrudServices;
        private readonly IVerifyUser VerifyUser;

        public SignUpUserController(IBaseCrudService<TemporaryUser,string> baseCrudServices, IVerifyUser verify)
        {
            BaseCrudServices = baseCrudServices;
            VerifyUser = verify;
        }



        // GET: api/<SignUpUserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SignUpUserController>/5
        [HttpGet("{email}")]
        public async Task<IActionResult> GetProfileData(string email)
        {
            try
            {
                if (BaseCrudServices == null)
                {
                    return BadRequest("BaseCrudServices is not initialized.");
                }

                var result = await BaseCrudServices.GetTheData(email);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("verifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody]OtpInfos otpModel)
        {
            var token = await VerifyUser.CheckOTPForEmailVerification(otpModel.otp,otpModel.email);
            if(token == null)
            {
                return BadRequest();
            }
            return Ok(token);

        }
        // POST api/<SignUpUserController>
        [HttpPost]
        public async Task<IActionResult> AddUserProfile([FromBody] TemporaryUser user)
        {
            try
            {
                var otp = await VerifyUser.VerifyEmailAddress(user);
             
                if (otp != null)
                {

                    return Ok(otp);
                }
                
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();
        }

        // PUT api/<SignUpUserController>/email
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string email, [FromBody] TemporaryUser user)
        {
            try
            {
                await BaseCrudServices.UpdateTheData(email, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<SignUpUserController>/email
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfileData(string email)
        {
            try
            {
                var result = await BaseCrudServices.DeleteTheData(email);

                return Ok(result);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
