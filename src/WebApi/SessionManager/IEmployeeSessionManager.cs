namespace WebApi.SessionManager;

public interface IEmployeeSessionManager
{
    string NewSession();
    bool Exists(string sessionId);
}