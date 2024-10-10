using ICMD.Core.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using ICMD.API.Helpers;
using ICMD.Core.Authorization;
using ICMD.Core.Constants;
using ICMD.Core.Dtos.Menu;

namespace ICMD.API.Auth
{
    public class TokenProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<JWTBearerDTO> _JWTconfig;
        public TokenProvider(IConfiguration configuration, IOptions<JWTBearerDTO> JWTconfig)
        {
            _configuration = configuration;
            _JWTconfig = JWTconfig;
        }

        public JwtToken GenerateTokenAsync(ClaimsPrincipal user, ICMDUser icmdUser, IList<string> roles, MenuAndPermissionListDto menuAndPermission = null)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:JwtBearer:SecurityKey"] ?? string.Empty));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenBuilder = new JwtTokenBuilder()
                  .AddSecurityKey(key)
                   .AddSubject(_JWTconfig.Value.Subject)
                  .AddIssuer(_JWTconfig.Value.Issuer)
                  .AddAudience(_JWTconfig.Value.Audience)
                  .AddClaim(IdentityClaimNames.UserId, icmdUser.Id.ToString(), roles, "")
                  .AddClaim(IdentityClaimNames.UserFullName, icmdUser.FullName ?? string.Empty, null, "")
                  .AddClaim(IdentityClaimNames.Email, icmdUser.Email ?? string.Empty, null, "")
                  .AddClaim(IdentityClaimNames.UserName, icmdUser.UserName ?? string.Empty, null, "")
                  .AddClaim(IdentityClaimNames.RoleName, roles != null ? roles[0] : "", null, "")
                  //.AddClaim(IdentityClaimNames.MenuAndPermission, menuAndPermission != null ? JsonConvert.SerializeObject(menuAndPermission, new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }) : "", null, "")
                  .AddExpiry(Convert.ToInt32(_configuration["Authentication:JwtBearer:ExpirationTimeInMinutes"]))
                  .Build();

            return tokenBuilder;
        }
    }
}
