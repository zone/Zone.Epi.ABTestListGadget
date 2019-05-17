using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Marketing.Testing.Core.DataClass;
using NSubstitute;
using NUnit.Framework;
using Zone.Epi.ABTestListGadget.Core.Services;

namespace Zone.Epi.ABTestListGadget.Tests
{
	[TestFixture]
	public class MarketingTestListServiceTests
	{
		private IContentRepository _stubContentRepository;
		private ISiteHelperService _stubSiteHelperService;
		private MarketingTestListService _marketingTestListService;

		[SetUp]
		public void SetUp()
		{
			_stubContentRepository = Substitute.For<IContentRepository>();
			_stubSiteHelperService = Substitute.For<ISiteHelperService>();

			_marketingTestListService = new MarketingTestListService(_stubContentRepository, _stubSiteHelperService);
		}

		[Test]
		public void ReturnPaginatedResults_WithSixItems_ReturnsPaginatedListWithOneItem()
		{
			// Arrange
			var subTestOne = Substitute.For<IMarketingTest>();
			var subTestTwo = Substitute.For<IMarketingTest>();
			var subTestThree = Substitute.For<IMarketingTest>();
			var subTestFour = Substitute.For<IMarketingTest>();
			var subTestFive = Substitute.For<IMarketingTest>();
			var subTestSix = Substitute.For<IMarketingTest>();
			subTestSix.Title = "Test Title";

			var testFullList = new List<IMarketingTest>
			{
				subTestOne, subTestTwo, subTestThree, subTestFour, subTestFive, subTestSix
			};

			// Act
			var result = _marketingTestListService.ReturnPaginatedResults(testFullList, 1);

			// Assert
			Assert.AreEqual(testFullList.Last().Title, result.First().Title);
		}

		[Test]
		public void ReturnPaginatedResultsForSite_WithSixItemsAndThreeForThisSite_ReturnsThreeItems()
		{
			// Arrange
			const string testSiteName = "Test Site";
			const string testTitle = "Test Title";
			const string allSiteName = "All";
			const int page = 0;

			var thisSiteValidParentRoot = new ContentReference(123);
			_stubSiteHelperService.GetSiteStartPageReference(testSiteName).Returns(thisSiteValidParentRoot);

			// Items
			var thisSiteValidItemRoot = new ContentReference(1234);
			var testSiteGuidOne = Guid.Parse("737bbab0-137e-43fb-af19-93b2b4a04cd5");
			var testSiteGuidTwo = Guid.Parse("f9e419c4-efc9-41f9-86ef-0096d81c8722");
			var testSiteGuidThree = Guid.Parse("7cd884ef-0963-4a61-b762-a9f5e3b21886");

			// These are setup to be "in" our filtered site
			var subTestOne = Substitute.For<IMarketingTest>();
			subTestOne.OriginalItemId = testSiteGuidOne;

			var subTestFive = Substitute.For<IMarketingTest>();
			subTestFive.OriginalItemId = testSiteGuidTwo;

			var subTestSix = Substitute.For<IMarketingTest>();
			subTestSix.OriginalItemId = testSiteGuidThree;
			subTestSix.Title = testTitle;

			_stubSiteHelperService.GetContentReferenceFromGuid(testSiteGuidOne).Returns(thisSiteValidItemRoot);
			_stubSiteHelperService.GetContentReferenceFromGuid(testSiteGuidTwo).Returns(thisSiteValidItemRoot);
			_stubSiteHelperService.GetContentReferenceFromGuid(testSiteGuidThree).Returns(thisSiteValidItemRoot);

			_stubSiteHelperService.CompareContentReferences(thisSiteValidItemRoot, thisSiteValidParentRoot).Returns(true);

			var validSiteParent = Substitute.For<IContent>();
			validSiteParent.ContentLink = thisSiteValidItemRoot;
			var validParentsList = new List<IContent> { validSiteParent };

			_stubContentRepository.GetAncestors(thisSiteValidItemRoot).Returns(validParentsList.Select(s => s));

			// these should be filtered out
			var subTestTwo = Substitute.For<IMarketingTest>();
			var subTestThree = Substitute.For<IMarketingTest>();
			var subTestFour = Substitute.For<IMarketingTest>();

			var testFullList = new List<IMarketingTest>
			{
				subTestOne, subTestTwo, subTestThree, subTestFour, subTestFive, subTestSix
			};

			// Act
			var result = _marketingTestListService.ReturnPaginatedResultsForSite(testFullList, testSiteName, page).ToList();
			var noSiteFilterResults = _marketingTestListService.ReturnPaginatedResultsForSite(testFullList, allSiteName, page);

			// Assert
			Assert.IsNotEmpty(result);
			Assert.AreEqual(testFullList[0], result[0]);
			Assert.AreEqual(testSiteGuidOne, result[0].OriginalItemId);
			Assert.AreEqual(testFullList[4], result[1]);
			Assert.AreEqual(testSiteGuidTwo, result[1].OriginalItemId);
			Assert.AreEqual(testFullList[5], result[2]);
			Assert.AreEqual(testSiteGuidThree, result[2].OriginalItemId);
			Assert.AreEqual(testTitle, result[2].Title);
			Assert.AreEqual(3, result.Count());

			Assert.IsNotEmpty(noSiteFilterResults);
			Assert.AreEqual(5, noSiteFilterResults.Count());
			Assert.AreEqual(testFullList.Take(5), noSiteFilterResults);
		}
	}
}