using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Zone.Epi.ABTestListGadget.Core.Services;

namespace Zone.Epi.ABTestListGadget.Core.Initialization
{
	[InitializableModule]
	public class DependencyInitialization : IConfigurableModule
	{
		public void ConfigureContainer(ServiceConfigurationContext context)
		{
			context.Services.AddTransient<IMarketingTestListService, MarketingTestListService>();
			context.Services.AddTransient<ISiteHelperService, SiteHelperService>();
		}

		public void Initialize(InitializationEngine context)
		{
		}

		public void Uninitialize(InitializationEngine context)
		{
		}
	}
}