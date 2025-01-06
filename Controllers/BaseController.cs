using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Generators;

namespace kursah_5semestr.Controllers
{
    [Route("/")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ISessionsService _sessionsService;
        private readonly ILogger _logger;

        public BaseController(IUsersService usersService, ISessionsService sessionsService, ILogger<BaseController> logger)
        { 
            _usersService = usersService;
            _sessionsService = sessionsService;
            _logger = logger;
        }

        private bool CheckBasicAuth()
        {
            var authHeader = Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader))
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var credentials = encoding.GetString(Convert.FromBase64String(authHeaderVal.Parameter));

                    int separator = credentials.IndexOf(':');
                    string name = credentials.Substring(0, separator);
                    string password = credentials.Substring(separator + 1);
                    var result = name == "admin" && _usersService.CheckPassword(name, password);
                    if (!result)
                    {
                        _logger.LogInformation($"Basic authentication failed for user '{name}'");
                    }
                    return result;
                }
            }
            return false;
        }

        [Route("/users")]
        [HttpPost]
        public async Task<ActionResult<UserOutDto>> PostUser(UserDto dto)
        {
            if (dto.Login != "admin" && !CheckBasicAuth()) 
            {
                return Unauthorized(new StatusOutDto("error"));
            }
            var user = new User();
            user.Login = dto.Login;
            user.PasswordHash = Convert.ToBase64String(BCrypt.PasswordToByteArray(dto.Password.ToCharArray()));
            var saved = await _usersService.CreateUser(user);
            var outDto = new UserOutDto(user.Id, user.Login);
            return CreatedAtAction(nameof(PostUser), new { id = user.Id }, outDto);
        }

        [Route("/login")]
        [HttpPost]
        public async Task<ActionResult<StatusOutDto>> Login(UserDto dto)
        {
            var session = await _usersService.Login(dto.Login, dto.Password);
            var outDto = new LoginOutDto(session.Token);
            return CreatedAtAction(nameof(Login), outDto);
        }

        [Route("/session/{token}")]
        [HttpGet]
        public ActionResult<UserOutDto> CheckSession(string token)
        {
            try
            {
                var session = _sessionsService.FindByToken(token);
                return Ok(new UserOutDto(session.User.Id, session.User.Login));
            }
            catch (Exception ex)
            {
                _logger.LogWarning("User session not found");
                return NotFound(new StatusOutDto("error"));
            }
        }
    }
}
