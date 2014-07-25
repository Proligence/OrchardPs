namespace Proligence.PowerShell.Controllers
{
    using System.Web.Mvc;
    using Proligence.PowerShell.Host;

    public class TestController : Controller
    {
        private readonly IPsSessionManager sessionManager;

        public TestController(IPsSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public ActionResult NewSession()
        {
            this.sessionManager.NewSession();
            return new EmptyResult();
        }
    }
}