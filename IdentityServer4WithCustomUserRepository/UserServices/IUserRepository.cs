using System.Threading.Tasks;

namespace CustomIdentityServer4.UserServices
{
    public interface IUserRepository
    {
        bool ValidateCredentials(string username, string password);

        Task<CustomUser> FindBySubjectId(string subjectId);

        CustomUser FindByUsername(string username);
    }
}
