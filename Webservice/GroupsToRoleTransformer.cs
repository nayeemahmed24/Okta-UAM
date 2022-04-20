using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace Webservice
{
    public class GroupsToRolesTransformer : IClaimsTransformation
    {
        private OktaClient client;
        private readonly IConfiguration configuration;

        public GroupsToRolesTransformer(IConfiguration configuration)
        {

            this.configuration = configuration;
            client = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = this.configuration["Okta:OktaDomain"],
                Token = this.configuration["Okta:Token"]
            });
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var idClaim = principal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            if (idClaim != null)
            {
                var user = await client.Users.GetUserAsync(idClaim.Value);
                if (user != null)
                {
                    var groups = user.Groups.ToEnumerable();
                    foreach (var group in groups)
                    {
                        ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Roles", group.Profile.Name));
                    }
                }
            }
            return principal;
        }
    }
}
