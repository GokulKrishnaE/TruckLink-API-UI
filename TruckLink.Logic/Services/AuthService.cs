using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using TruckLink.Core.Entities;
using TruckLink.Core.Interfaces;

namespace TruckLink.Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IRepository<UserRoleMapping> _userRoleRepo;
        private readonly IConfiguration _config;

        public AuthService(
            IRepository<User> userRepo,
            IRepository<Role> roleRepo,
            IRepository<UserRoleMapping> userRoleRepo,
            IConfiguration config)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _config = config;
        }

        public async Task<bool> RegisterAsync(string name, string email, string password)
        {
            var existingUser = await _userRepo.GetFirstAsync(u => u.Email == email);

            if (existingUser != null)
                return false;

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return true;
        }

        public async Task<(string accessToken,string refreshToken)?> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetFirstAsync(u => u.Email == email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            var mapping = await _userRoleRepo.GetFirstAsync(x => x.UserId == user.Id);

            var role = mapping != null
                ? await _roleRepo.GetFirstAsync(r => r.Id == mapping.RoleId)
                : null;

            var accessToken = GenerateJwtToken(user, role?.Name);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }

        private string GenerateJwtToken(User user, string? role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var expireHours = double.TryParse(_config["Jwt:ExpireHours"], out var hours)
                ? hours
                : 8;

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<string> refresh(string refreshToken)
        {
            var user = await _userRepo.GetFirstAsync(x => x.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return null;

            var mapping = await _userRoleRepo.GetFirstAsync(x => x.UserId == user.Id);
            var role = mapping != null
                ? await _roleRepo.GetFirstAsync(r => r.Id == mapping.RoleId)
                : null;

            var newAccessToken = GenerateJwtToken(user, role?.Name);

            return newAccessToken;
        }


        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var mappings = await _userRoleRepo.GetAllAsync(x => x.UserId == userId);

            if (mappings == null || !mappings.Any())
                return new List<string>();

            var roleIds = mappings.Select(x => x.RoleId).ToList();

            var roles = await _roleRepo.GetAllAsync(r => roleIds.Contains(r.Id));

            return roles.Select(r => r.Name).ToList();
        }
    }
}