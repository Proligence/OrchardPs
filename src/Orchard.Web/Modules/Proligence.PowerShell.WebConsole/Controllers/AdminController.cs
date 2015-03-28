namespace Proligence.PowerShell.WebConsole.Controllers
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
            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Console()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageSettings, T("Not authorized to access console.")))
            {
                return new HttpUnauthorizedResult();
            }

            return View();
        }
    }
}