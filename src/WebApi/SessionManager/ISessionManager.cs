namespace WebApi.SessionManager;

public interface ISessionManager
{
    string NewSession();
    bool Exists(string sessionId);
}