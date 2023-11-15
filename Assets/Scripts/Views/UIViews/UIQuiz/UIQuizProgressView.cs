using UnityEngine;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizProgressView : BaseView
    {
        [Header("Objects")]
        public GameObject progressItem;

        [Header("UI")]
        public Transform progressArea;

        public void LoadView()
        {

        }

        public void RemoveView()
        {

        }

        public void InitProgress(byte count)
        {
            foreach (Transform child in progressArea)
            {
                Destroy(child.gameObject);
            }

            GameObject item;
            for (byte i = 0; i < count; i++)
            {
                item = Instantiate(progressItem, progressArea);
                SwitchItem(item, switchOn: false);
            }
        }

        public void SetCurrentProgress(byte current)
        {
            byte i = 0;
            foreach (Transform child in progressArea)
            {
                SwitchItem(child.gameObject, switchOn: i <= current);
                i++;
            }
        }

        private void SwitchItem(GameObject item, bool switchOn)
        {
            Image im = item.GetComponent<Image>();
            if (im)
            {
                im.enabled = !switchOn;
            }

            Transform tr = item.transform.GetChild(0);
            if (tr)
            {
                tr.gameObject.SetActive(switchOn);
            }
        }
    }
}