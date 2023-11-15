using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.context.api;
using PFS.Assets.Scripts.Services.Localization;

public class BaseView : View
{
	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher Dispatcher { get; set;}

    public object otherData = null;

    [Inject]
    public LocalizationManager LocalizationManager { get; set; }

}