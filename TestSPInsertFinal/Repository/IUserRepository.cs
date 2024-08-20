using Microsoft.AspNetCore.Mvc;
using TestSPInsertFinal.Models;

namespace TestSPInsertFinal.Repository
{
    public interface IUserRepository
    {
        string CreateToken(string email);
        Task<int> DeleteUserDetails(string email);
        Task<IEnumerable<RegisterUser>> GetAllUserDetails();
        Task<bool> GetEmailExists(string email);
        Task<IEnumerable<RegisterUser>> GetUserDetails(string email);
        Task<string> GetUserPassword(string email);
        Task<bool> GetUserStatus(string email);
        Task<int> InsertUserDetails([FromBody] RegisterUser userData);
        Task<int> UpdateUserDetails(UserDetails userData, string email);
    }
}