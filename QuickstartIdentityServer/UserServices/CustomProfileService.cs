using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4;
using Microsoft.Extensions.Logging;

namespace CustomIdentityServer4.UserServices
{
    public class CustomProfileService : IProfileService
    {
        protected readonly ILogger Logger;


        protected readonly CustomUserStore Users;

        public CustomProfileService(CustomUserStore users, ILogger<CustomProfileService> logger)
        {
            Users = users;
            Logger = logger;
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);

            var user = Users.FindBySubjectId(context.Subject.GetSubjectId());

            //if (context.RequestedClaimTypes.Any())
            //{            
            //    context.AddFilteredClaims(user.Claims);
            //}


            user.Claims.Add(new Claim("role", "dataEventRecords.admin"));
            user.Claims.Add(new Claim("role", "dataEventRecords.user"));

            context.IssuedClaims = user.Claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            context.IsActive = user != null;
        }
    }
}