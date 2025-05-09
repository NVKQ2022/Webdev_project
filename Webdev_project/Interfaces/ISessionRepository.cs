using Webdev_project.Models;

namespace Webdev_project.Interfaces
{
    public interface ISessionRepository
    {
        string CreateSession(User user, string ipAddress, DateTime requestTime, string userAgent);
    }
}
