namespace WebApi.SessionManager;

public interface ISessionManager
{
    string NewSession();
    bool IsLoggedIn(string sessionId);
}