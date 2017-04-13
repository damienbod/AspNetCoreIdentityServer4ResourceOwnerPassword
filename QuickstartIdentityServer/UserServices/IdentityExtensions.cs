using CustomIdentityServer4.UserServices;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomIdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddCustomUsers(this IIdentityServerBuilder builder, List<CustomUser> users)
        {
            builder.Services.AddSingleton(new CustomUserStore(users));
            builder.AddProfileService<CustomProfileService>();
            builder.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}