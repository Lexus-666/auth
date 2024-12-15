using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using kursah_5semestr.Abstractions;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace kursah_5semestr.Services
{
    public class SessionsService : ISessionsService
    {
        // private const string CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string KEY = "Bkwnhxz1jRyaJ8OBmS3YuJfGVvg13UYP5iQt8pluGuzOOHpWWthMcBJkwGYX89Mu";
        private const string ISSUER = "my-auth-service";
        private const string AUDIENCE = "my-application";
        private const int TTL = 1000;
        private AppDbContext _context;

        public SessionsService(AppDbContext context)
        {
            _context = context;
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
                };
            var jwt = new JwtSecurityToken(
                    issuer: ISSUER,
                    audience: AUDIENCE,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(TTL)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public async Task<Session> CreateSession(User user)
        {
            var session = new Session();
            session.Id = Guid.NewGuid();
            session.User = user;
            session.Token = GenerateToken(user);
            _context.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public Session FindByToken(string token)
        {
            var session = _context.Sessions.Include(s => s.User).Where(s => s.Token == token).First();
            return session;
        }
    }
}
