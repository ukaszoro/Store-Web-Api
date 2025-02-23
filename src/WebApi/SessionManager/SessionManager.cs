namespace WebApi.SessionManager;

public class SessionManager : ISessionManager, IUserSessionManager
{
    private readonly HashSet<string> _sessions = new();

    public string NewSession()
    {
        var sessionId = Guid.NewGuid().ToString();
        _sessions.Add(sessionId);
        return sessionId;
    }

    public bool Exists(string sessionId)
    {
        return _sessions.Contains(sessionId);
    }
}