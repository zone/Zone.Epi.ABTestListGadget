using EPiServer.Shell.ViewComposition;

namespace Zone.Epi.ABTestListGadget
{
    [Component(
		Title = "Active A/B Tests",
		Categories = "dashboard",
		WidgetType = "zoneabtestlistcomponent/zoneabtestlistviewer",
		Description = "Review running A/B tests"
	)]
	public class ABTestListComponent
	{
	}
}
