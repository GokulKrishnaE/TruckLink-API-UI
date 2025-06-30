using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLink.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string name, string email, string password, string role);
        Task<string?> LoginAsync(string email, string password);
    }
}
