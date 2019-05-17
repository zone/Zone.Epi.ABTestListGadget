using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Marketing.Testing.Core.DataClass;
using EPiServer.Web;

namespace Zone.Epi.ABTestListGadget.Core.Services
{
	public class MarketingTestListService : IMarketingTestListService
	{
		private IContentRepository _contentRepository;
		private ISiteHelperService _siteHelperService;

		public MarketingTestListService(IContentRepository contentRepository, ISiteHelperService siteHelperService)
		{
			_contentRepository = contentRepository;
			_siteHelperService = siteHelperService;
		}

		public IEnumerable<IMarketingTest> ReturnPaginatedResults(IEnumerable<IMarketingTest> list, int page)
		{
			return list.Skip(page * 5)?.Take(5);
		}

		public IEnumerable<IMarketingTest> ReturnPaginatedResultsForSite(IEnumerable<IMarketingTest> list, string site, int page)
		{
			if (string.IsNullOrWhiteSpace(site) || string.Equals(site, Constants.AllFilterKey, StringComparison.OrdinalIgnoreCase))
				return ReturnPaginatedResults(list, page);

			// Get the site reference to filter on
			var siteParentRoot = _siteHelperService.GetSiteStartPageReference(site);

			var filteredResults = new List<IMarketingTest>();
			var neededResults = (page + 1) * 5;

			// Filter the results
			foreach (var test in list)
			{
				var contentReference = _siteHelperService.GetContentReferenceFromGuid(test.OriginalItemId);
				var parents = _contentRepository.GetAncestors(contentReference);
				if (parents.Any(w => _siteHelperService.CompareContentReferences(w.ContentLink, siteParentRoot)))
					filteredResults.Add(test);

				// Break early if required paged amount is found
				if (filteredResults.Count >= neededResults)
					break;
			}

			return ReturnPaginatedResults(filteredResults, page);
		}
	}
}