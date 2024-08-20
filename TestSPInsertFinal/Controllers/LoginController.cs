using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestSPInsertFinal.Models;
using TestSPInsertFinal.Repository;

namespace TestSPInsertFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailRepository emailRepository;
        private readonly IConfiguration configuration;
        private readonly ILogger<LoginController> logger;

        public LoginController(IUserRepository userRepository,IEmailRepository emailRepository, ILogger<LoginController> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.emailRepository= emailRepository;
        }

        [HttpPost("login")]

        public async Task<ActionResult<User>> Login([FromBody]Login login)
        {
            try
            {
                if (emailRepository.IsValidEmail(login.email))
                {
                    var inPassword = login.password;
                    bool result = await userRepository.GetUserStatus(login.email);
                    if (result == true)
                    {
                        var acPassword = await userRepository.GetUserPassword(login.email);
                        if (inPassword == acPassword)
                        {
                            string token = userRepository.CreateToken(login.email);

                            return Ok(new
                            {
                                status = "200",
                                message = "success",
                                data = new
                                {
                                    message = $"Your JWT token: {token}",
                                }
                            });

                        }
                        else
                        {
                            return BadRequest(new
                            {
                                status = "401",
                                message = "Wrong Password",
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            status = "404",
                            message = "User does not exist",
                        });
                    }
                }
                else
                {

                    return BadRequest(new
                    {
                        status = "400",
                        message = "Email ID is not valid",
                    });
                }
            }         
            catch (Exception ex)
            {
                logger.LogError("Error at LoginController.Login: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("Forgot Password")]

        public async Task<IActionResult> sendEmailPassword([FromQuery]Email mail)
        {
            try
            {
                if(emailRepository.IsValidEmail(mail.email)){
                    string password = await userRepository.GetUserPassword(mail.email);
                    emailRepository.sendPassword(mail.email, password);
                    return Ok(new
                    {
                        status = "200",
                        message = "Your password has been sent to your registered email id",
                    });
                }
                else
                {

                    return BadRequest(new
                    {
                        status = "400",
                        message = "Email ID is not valid",
                    });
                }
            }
            catch(Exception ex)
            {
                logger.LogError("Error at LoginController.sendEmailPassword: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        
    }
}
