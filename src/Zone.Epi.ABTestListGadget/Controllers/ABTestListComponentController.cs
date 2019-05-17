using System.Linq;
using System.Web.Mvc;
using EPiServer.Marketing.Testing.Core.Manager;
using EPiServer.Shell;
using Zone.Epi.ABTestListGadget.Core.Services;

namespace Zone.Epi.ABTestListGadget.Controllers
{
	[Authorize(Roles = "WebEditors, WebAdmins, Administrators")]
	public class ABTestListComponentController : Controller
	{
		private ITestManager _testManager;
		private IMarketingTestListService _marketingTestListService;
		private ISiteHelperService _siteHelperService;

		public ABTestListComponentController(ITestManager testManager, IMarketingTestListService marketingTestListService,
			ISiteHelperService siteHelperService)
		{
			_testManager = testManager;
			_marketingTestListService = marketingTestListService;
			_siteHelperService = siteHelperService;
		}

		public PartialViewResult Index()
		{
			// This only returns the Index which contains the Dojo template
			return PartialView(Paths.ToClientResource("Zone.Epi.ABTestListGadget", "Views") + "/Index.cshtml");
		}

		public JsonResult GetModel(int page = 0, string filterOnSitename = "")
		{
			if (page < 0)
				page = 0;

			// Get all active tests (this is the part where we wish for a filter API instead of getting all)
			var allActiveTests = _testManager.GetActiveTests()?.OrderBy(o => o.EndDate).Select(s => s);
			if (allActiveTests == null)
				return null;

			// Filter results
			var filteredTests = _marketingTestListService.ReturnPaginatedResultsForSite(allActiveTests, filterOnSitename, page);

			var cmsUrl = _siteHelperService.GetSiteCmsUrl();
			var sites = _siteHelperService.GetActiveSites();

			// Return the Json object our zoneabtestlistviewer.js expects
			return Json(new
			{
				ActiveTestList = filteredTests.Select(s => new
				{
					Title = s.Title?.Substring(0, s.Title.IndexOf(" A/B Test")),
					//Link = Url.ContentUrl(PermanentLinkUtility.FindContentReference(s.OriginalItemId)), <- links to the content directly
					Link = $"{cmsUrl}#context=epi.marketing.testing:///testid={s.Id}/details",
					StartedBy = s.Owner,
					StartDate = s.StartDate.ToString("dd-MM-yyyy"),
					EndDate = s.EndDate.ToString("dd-MM-yyyy"),
					ParticipationPercentage = $"{s.ParticipationPercentage.ToString()}%",
					Views = $"{s.Variants[0]?.Views} / {s.Variants[1]?.Views}",
					Conversions = $"{s.Variants[0]?.Conversions} / {s.Variants[1]?.Conversions}"
				}),
				Page = page,
				Sites = sites?.Count > 2 ? sites : null
			}, JsonRequestBehavior.AllowGet);
		}
	}
}