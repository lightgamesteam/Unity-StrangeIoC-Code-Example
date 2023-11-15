using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PFS.Assets.Scripts.Views.QuizStats
{
    public class StatGraphColumn : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI day;
        [SerializeField] private Image imageTarget;
        [SerializeField] private GameObject valueContainer;
        [SerializeField] private TextMeshProUGUI value;

        public void Initialize(int dataValue, int dataDay)
        {
            day.text = dataDay + "";
            value.text = dataValue + "";

            float size = dataValue / 10;
            if (size == 0)
            {
                size = 0.05f;
                value.color = new Color32(171, 171, 171, 255);
            }

            imageTarget.fillAmount = size;
            valueContainer.transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(valueContainer.transform.GetComponentInChildren<RectTransform>().sizeDelta.x, imageTarget.transform.GetComponentInChildren<RectTransform>().sizeDelta.y * size);
        }
        //public void UpdateTable()
        //{
        //    // Destroy old columns
        //    List<GameObject> children = new List<GameObject>();
        //    foreach (Transform child in diagramContent.transform) 
        //    { 
        //        children.Add(child.gameObject);
        //    } 
        //    children.ForEach(child => Destroy(child));

        //    // Create new columns
        //    for (int i = 0; i < QuizStatModel.week.Length; i++)
        //    {
        //        GameObject point = Instantiate(diagramItem) as GameObject;

        //        if (diagramContent != null)
        //        {

        //            point.transform.SetParent(diagramContent.transform, false);
        //            // Set size for column

        //            float 
        //            float size = QuizStatModel.week[i].value / 10;
        //            if (size == 0)
        //            {
        //                size = 0.2f;
        //            }

        //            Image tempcolumn = point.GetComponentInChildren<Image>();
        //            tempcolumn.fillAmount = size;

        //            //switch (QuizStatModel.week[i].value)
        //            //{
        //            //    case 0:
        //            //        Image tempcolumn = point.GetComponentInChildren<Image>();
        //            //        tempcolumn.transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -14);
        //            //        tempcolumn.transform.GetComponentsInChildren<RectTransform>()[1].anchoredPosition = new Vector2(5.054474e-05f, 18.9f);
        //            //        break;
        //            //    case 1:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -29);
        //            //        break;
        //            //    case 2:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -58);
        //            //        break;
        //            //    case 3:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -87);
        //            //        break;
        //            //    case 4:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -116);
        //            //        break;
        //            //    case 5:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -145);
        //            //        break;
        //            //    case 6:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -174);
        //            //        break;
        //            //    case 7:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -203);
        //            //        break;
        //            //    case 8:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -232);
        //            //        break;
        //            //    case 9:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -261);
        //            //        break;
        //            //    case 10:
        //            //        point.GetComponentInChildren<Image>().transform.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(-1, -290);
        //            //        break;
        //            //}

        //            point.GetComponentsInChildren<TextMeshProUGUI>()[0].text = QuizStatModel.week[i].value + "";
        //            point.GetComponentsInChildren<TextMeshProUGUI>()[1].text = QuizStatModel.week[i].day + "";
        //        }
        //    }
        //}
    }
}