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
        private const string ISSUER = "my-auth-service";
        private const string AUDIENCE = "my-application";
        private const int TTL = 1000;
        private readonly ISessionsRepository _sessionsRepository;
        private readonly ILogger _logger;
        private string _key;

        public SessionsService(ISessionsRepository sessionsRepository, ILogger<SessionsService> logger)
        {
            _sessionsRepository = sessionsRepository;
            _logger = logger;
            _key = File.ReadAllText("./key.dat");
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
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
            session.User = user;
            session.Token = GenerateToken(user);
            session = await _sessionsRepository.CreateSession(session);
            _logger.LogInformation($"Created session {session.Id}");
            return session;
        }

        public Session FindByToken(string token)
        {
            return _sessionsRepository.FindByToken(token);
        }
    }
}
