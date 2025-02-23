namespace WebApi.SessionManager;

public interface IUserSessionManager
{
    string NewSession();
    bool Exists(string sessionId);
}