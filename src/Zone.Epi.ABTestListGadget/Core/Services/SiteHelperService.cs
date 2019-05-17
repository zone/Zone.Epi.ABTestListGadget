using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Web;

namespace Zone.Epi.ABTestListGadget.Core.Services
{
	public class SiteHelperService : ISiteHelperService
	{
		private ISiteDefinitionRepository _siteDefinitionRepository;

		public SiteHelperService(ISiteDefinitionRepository siteDefinitionRepository)
		{
			_siteDefinitionRepository = siteDefinitionRepository;
		}

		public List<string> GetActiveSites()
		{
			var siteDefinitionNames = _siteDefinitionRepository.List().Select(x => x.Name);

			var resultList = new List<string> { Constants.AllFilterKey };
			resultList.AddRange(siteDefinitionNames);

			return resultList;
		}

		public string GetSiteCmsUrl()
		{
			var cmsUrl = Settings.Instance.UIUrl.ToString();
			if (cmsUrl.StartsWith("~/"))
				cmsUrl = cmsUrl.Substring(2);

			return cmsUrl;
		}

		public ContentReference GetSiteStartPageReference(string siteName)
		{
			return _siteDefinitionRepository.List()?.Where(w => w.Name == siteName)?.Select(s => s.StartPage)?.FirstOrDefault();
		}

		public ContentReference GetContentReferenceFromGuid(Guid guid)
		{
			return PermanentLinkUtility.FindContentReference(guid);
		}

		public bool CompareContentReferences(ContentReference firstItem, ContentReference secondItem)
		{
			return firstItem.CompareToIgnoreWorkID(secondItem);
		}
	}
}