using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User? AuthenticateUser(string email, string password);
    }

}
