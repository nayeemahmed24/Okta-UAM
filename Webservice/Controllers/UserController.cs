using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webservice.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private OktaClient client;
        private readonly IConfiguration configuration;

        public UserController(IConfiguration configuration)
        {

            this.configuration = configuration;
            client = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = this.configuration["Okta:OktaDomain"],
                Token = this.configuration["Okta:Token"]
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(Models.User user)
        {
                      
            var res = await client.Users.CreateUserAsync(new CreateUserWithPasswordOptions
            {
                Profile = new UserProfile
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Login = user.UserName,
                    MobilePhone = user.MobilePhone
                },
                Activate = false,
                Password = user.Password
            });


            var existingUser = await client.Users.GetUserAsync(user.UserName);
            var group = await client.Groups.FirstOrDefaultAsync(x => x.Profile.Name == "Modarator");

            if (group != null && user != null)
            {
                await client.Groups.AddUserToGroupAsync(group.Id, existingUser.Id);
            }

            return this.Ok(existingUser);
        }


        public async Task<IActionResult> ActiveUser(string username)
        {
            var existingUser = await client.Users.GetUserAsync(username);
            var res = await client.Users.ActivateUserAsync(username, false);
            return this.Ok(res);
        }

        public async Task<IActionResult> DeactiveUser(string username)
        {
            var existingUser = await client.Users.GetUserAsync(username);
            await client.Users.DeactivateOrDeleteUserAsync(existingUser.Id);
            return this.Ok("Deactivated Successfully");
        }

        public async Task<IActionResult> AssignAdminRole(string userId)
        {
            var groups = client.Groups.ListGroups();
            var group = await groups.Where(p => p.Profile.Name == "Admin").FirstOrDefaultAsync();
            await client.Groups.AddUserToGroupAsync(group.Id, userId);
            return this.Ok("Assigned Admin Role Successfully");
        }

        public async Task<IActionResult> GetUser(string username)
        {
            var user = await client.Users.GetUserAsync(username);
            return this.Ok(user);
        }
    }
}
