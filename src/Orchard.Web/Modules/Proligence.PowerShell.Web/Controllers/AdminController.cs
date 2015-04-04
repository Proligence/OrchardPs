namespace Proligence.PowerShell.Web.Controllers
{
    using System.Web.Mvc;
    using Orchard;
    using Orchard.Core.Settings;
    using Orchard.Environment.Extensions;
    using Orchard.Localization;

    [ValidateInput(false)]
    [OrchardFeature("Proligence.Powershell.WebConsole")]
    public class AdminController : Controller 
    {
        public AdminController(IOrchardServices services) 
        {
            this.Services = services;
            this.T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Console()
        {
            if (!this.Services.Authorizer.Authorize(Permissions.ManageSettings, this.T("Not authorized to access console.")))
            {
                return new HttpUnauthorizedResult();
            }

            return this.View();
        }
    }
}