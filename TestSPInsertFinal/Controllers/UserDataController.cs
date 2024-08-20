using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestSPInsertFinal.Models;
using TestSPInsertFinal.Repository;

namespace TestSPInsertFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailRepository emailRepository;
        private readonly ILogger<UserDataController> logger;
        public UserDataController(IUserRepository userRepository,IEmailRepository emailRepository ,ILogger<UserDataController> logger)
        {
            this.userRepository = userRepository;
            this.emailRepository = emailRepository;
            this.logger = logger;
        }


        [HttpGet, Authorize(Roles = "User")]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails([FromQuery] Email mail)
        {
            try
            {
                if (emailRepository.IsValidEmail(mail.email))
                {
                    bool result = await userRepository.GetUserStatus(mail.email);
                    if (result == true)
                    {
                        var user = await userRepository.GetUserDetails(mail.email);
                        return Ok(new
                        {
                            status = "200",
                            message = "success",
                            data = new
                            {
                                user
                            }
                        });
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
                logger.LogError("Error at UserDataController.GetUserDetails: " + ex.Message);
                throw new Exception(ex.Message);
            }

        }

        [HttpPut, Authorize(Roles = "User")]
        [Route("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserData([FromBody] UserDetails userData)
        {
            try
            {
                if (emailRepository.IsValidEmail(userData.email))
                {
                    bool result = await userRepository.GetUserStatus(userData.email);

                    if (result == true)
                    {
                        await userRepository.UpdateUserDetails(userData, userData.email);
                        return Ok(new
                        {
                            status = "200",
                            message = "success",
                            data = new
                            {
                                userData
                            }
                        });

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
                logger.LogError("Error at UserDataController.UpdateUserData: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete, Authorize(Roles = "User")]
        [Route("DeactivateUser")]
        public async Task<IActionResult> DeleteUser([FromBody] Email mail)
        {
            try
            {
                if (emailRepository.IsValidEmail(mail.email))
                {
                    bool result = await userRepository.GetUserStatus(mail.email);
                    if (result == true)
                    {
                        await userRepository.DeleteUserDetails(mail.email);
                        return Ok(new
                        {
                            status = "200",
                            message = "successfully deactivated the user",
                        }
                        );
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
                logger.LogError("Error at UserDataController.DeleteUser: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        

    }
}
