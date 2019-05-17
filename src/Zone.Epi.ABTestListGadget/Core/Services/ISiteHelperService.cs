using System;
using System.Collections.Generic;
using EPiServer.Core;

namespace Zone.Epi.ABTestListGadget.Core.Services
{
	public interface ISiteHelperService
	{
		List<string> GetActiveSites();

		ContentReference GetSiteStartPageReference(string siteName);

		string GetSiteCmsUrl();

		ContentReference GetContentReferenceFromGuid(Guid guid);
		
		bool CompareContentReferences(ContentReference firstItem, ContentReference secondItem);
	}
}