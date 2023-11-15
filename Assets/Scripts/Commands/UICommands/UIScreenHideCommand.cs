using PFS.Assets.Scripts.Models.ScreenManagerModels;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.UI
{
	public class UIScreenHideCommand : BaseCommand
	{
		string nameScreen = "";

		public override void Execute()
		{
			if (EventData.data != null)
			{
				ShowScreenModel ssm = EventData.data as ShowScreenModel;
				if (ssm != null)
				{
					nameScreen = ssm.screenName;
				}
				else
				{
					nameScreen = EventData.data.ToString();
				}
			}
			else
			{
				Debug.LogError("No Name Screen");
			}

			Object obj = GameObject.Find(nameScreen);
			if (!obj)
			{
				Debug.LogWarning("GameObject.Find " + nameScreen + " == NULL");
			}
			else
			{
				// Fix for Step's views
				//((GameObject)obj).GetComponent<Canvas>().enabled = false;
				obj.name = obj.name + "(ToDelete)";

				var mainCanvas = ((GameObject)obj).GetComponent<Canvas>();
				if (mainCanvas != null)
					mainCanvas.enabled = false;

				var allCanvases = ((GameObject)obj).GetComponentsInChildren<Canvas>();
				foreach (var curCanvas in allCanvases)
					curCanvas.enabled = false;

				GameObject.Destroy(obj);
				obj = null;
			}
		}
	}
}