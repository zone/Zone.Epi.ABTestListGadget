using System.Collections.Generic;
using EPiServer.Marketing.Testing.Core.DataClass;

namespace Zone.Epi.ABTestListGadget.Core.Services
{
	public interface IMarketingTestListService
	{
		IEnumerable<IMarketingTest> ReturnPaginatedResults(IEnumerable<IMarketingTest> list, int page);

		IEnumerable<IMarketingTest> ReturnPaginatedResultsForSite(IEnumerable<IMarketingTest> list, string site, int page);
	}
}