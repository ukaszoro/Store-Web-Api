using NUnit.Framework;
using WebApi.SessionManager;

namespace tests.WebApiTests;

[TestFixture]
public class SessionManagerTest
{
    [Test]
    public void CreateSessionTest()
    {
        var sessionManager = new SessionManager();
        var session1 = sessionManager.NewSession();
        Assert.IsNotNull(session1);
        Assert.True(sessionManager.Exists(session1));
        
        var session2 = sessionManager.NewSession();
        Assert.IsNotNull(session2);
        Assert.True(sessionManager.Exists(session2));
        
        var session3 = sessionManager.NewSession();
        Assert.IsNotNull(session3);
        Assert.True(sessionManager.Exists(session3));
    }
}