using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestSPInsertFinal.Models;

namespace TestSPInsertFinal.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _config;
        private readonly SqlConnection connection;
        private readonly IPasswordRepository passwordRepository;
        private readonly ILogger<UserRepository> logger;
        public UserRepository(IConfiguration config, IPasswordRepository passwordRepository, ILogger<UserRepository> logger)
        {
            this.passwordRepository = passwordRepository;
            _config = config;
            connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            this.logger = logger;
        }
        public async Task<IEnumerable<RegisterUser>> GetAllUserDetails()
        {
            try
            {
                var result = await connection.QueryAsync<RegisterUser>("GetUserData", commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.GetAllUserDetails: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> InsertUserDetails([FromBody] RegisterUser userData)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@firstName", userData.firstName);
                parameters.Add("@lastName", userData.lastName);
                parameters.Add("@email", userData.email);
                string encryptedPassword = passwordRepository.EncryptString(userData.password);
                parameters.Add("password", encryptedPassword);
                Guid newGuid = Guid.NewGuid();
                parameters.Add("@userID", newGuid);
                parameters.Add("@address", userData.address);
                parameters.Add("@city", userData.city);
                parameters.Add("@state", userData.state);
                parameters.Add("@postalCode", userData.postalCode);
                parameters.Add("@DOB", userData.DOB);
                parameters.Add("@age", userData.age);
                parameters.Add("@contactNumber", userData.contactNumber);

                int result = await connection.ExecuteAsync("InsertUserData", parameters, commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.InsertlUserDetails: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> GetUserStatus(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email);
                parameters.Add("@userActive", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("CheckUserActive", parameters, commandType: CommandType.StoredProcedure);
                bool userActive = parameters.Get<bool>("@userActive");
                return userActive;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.GetUserStatus: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<RegisterUser>> GetUserDetails(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email);
                var result = await connection.QueryAsync<RegisterUser>("GetOneUserData", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.GetUserDetails: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> GetEmailExists(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email);
                parameters.Add("@emailExists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("CheckEmailExists", parameters, commandType: CommandType.StoredProcedure);
                bool emailExists = parameters.Get<bool>("@emailExists");
                return emailExists;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.GetEmailExists: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> DeleteUserDetails(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email);
                var result = await connection.ExecuteAsync("DeactivateUser", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.GetEmailExists: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateUserDetails(UserDetails userData, string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email);
                parameters.Add("@address", userData.address);
                parameters.Add("@city", userData.city);
                parameters.Add("@state", userData.state);
                parameters.Add("@postalcode", userData.postalCode);
                parameters.Add("DOB", userData.DOB);
                parameters.Add("@age", userData.age);
                parameters.Add("@contactNumber", userData.contactNumber);
                var result = await connection.ExecuteAsync("UpdateUserDetails", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.UpdateUserDetails: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public async Task<string> GetUserPassword(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email);
                parameters.Add("@password", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
                await connection.ExecuteAsync("GetUserPassword", parameters, commandType: CommandType.StoredProcedure);
                string encryptedResult = parameters.Get<string>("@password");
                string result = passwordRepository.DecryptString(encryptedResult);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.GetUserPassword: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public string CreateToken(string email)
        {
            try
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Role,"User")
            };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _config.GetSection("AppSettings:Token").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: creds);

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt;
            }
            catch (Exception ex)
            {
                logger.LogError("Error at UserRepository.CreateToken: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

    }
}

