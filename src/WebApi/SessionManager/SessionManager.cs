namespace WebApi.SessionManager;

public class SessionManager : ISessionManager
{
    private readonly HashSet<string> _sessions = new();

    public string NewSession()
    {
        var sessionId = Guid.NewGuid().ToString();
        _sessions.Add(sessionId);
        return sessionId;
    }

    public bool IsLoggedIn(string sessionId)
    {
        return _sessions.Contains(sessionId);
    }
}