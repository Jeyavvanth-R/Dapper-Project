using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestSPInsertFinal.Models;
using TestSPInsertFinal.Repository;

namespace TestSPInsertFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<RegisterController> logger;
        private readonly IEmailRepository emailRepository;
        public RegisterController(IUserRepository userRepository,IEmailRepository emailRepository ,ILogger<RegisterController> logger)
        {
            this.userRepository = userRepository;
            this.emailRepository=emailRepository;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserData([FromBody] RegisterUser userData)
        {
            try
            {
                string email = userData.email;
                if (emailRepository.IsValidEmail(userData.email))
                {
                    bool result = await userRepository.GetEmailExists(email);
                    if (result == false)
                    {
                        await userRepository.InsertUserDetails(userData);
                        logger.LogInformation("New User has been added: " + email);
                        string token = userRepository.CreateToken(email);
                        return Ok(new
                        {
                            status = "200",
                            message = "success",
                            data = new
                            {
                                message = $"Your JWT token: {token}",
                                userData
                            }
                        });
                    }

                    else
                    {
                        return BadRequest(new
                        {
                            status = "409",
                            message = "Email ID already exists",
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
                logger.LogError("Error at RegisterController.PostUserData: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }

   
}
