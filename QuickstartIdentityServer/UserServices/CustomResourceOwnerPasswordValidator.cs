using IdentityServer4.Validation;
using IdentityModel;
using System.Threading.Tasks;

namespace CustomIdentityServer4.UserServices
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly CustomUserStore _users;

        public CustomResourceOwnerPasswordValidator(CustomUserStore users)
        {
            _users = users;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_users.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _users.FindByUsername(context.UserName);
                context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            }

            return Task.FromResult(0);
        }
    }
}
