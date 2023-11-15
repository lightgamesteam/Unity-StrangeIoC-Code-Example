using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.command.impl;
using PFS.Assets.Scripts.Services.Localization;

namespace PFS.Assets.Scripts.Commands
{
    public abstract class BaseCommand : Command
    {
        //[Inject]
        //public IMediationBinder MediationBinder { get; private set;}

        [Inject(ContextKeys.CONTEXT_DISPATCHER)]
        public IEventDispatcher Dispatcher { get; set; }

        [Inject]
        public LocalizationManager LocalizationManager { get; set; }


        [Inject]
        public IEvent EventData { get; set; }


        public override void Retain()
        {
            base.Retain();
        }

        public override void Release()
        {
            base.Release();
        }
    }
}