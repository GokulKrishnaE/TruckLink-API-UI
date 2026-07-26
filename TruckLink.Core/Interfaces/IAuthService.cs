using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLink.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string name, string email, string password);
        Task<(string accessToken, string refreshToken)?> LoginAsync(string email, string password);
        Task<string> refresh(string refreshToken);
        Task<List<string>> GetUserRolesAsync(int userId);
    }
}
