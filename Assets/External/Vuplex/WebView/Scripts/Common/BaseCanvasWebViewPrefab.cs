using UnityEngine;
using UnityEngine.UI;

namespace Vuplex.WebView
{
    public class BaseCanvasWebViewPrefab : MonoBehaviour
    {
        [Header("WebView")]
        public CanvasWebViewPrefab canvasWebViewPrefab;
        
        [Header("UI")]
        public Button closeButton;

        private void OnValidate()
        {
            canvasWebViewPrefab = GetComponentInChildren<CanvasWebViewPrefab>(true);
        }

        private void Start()
        {
            closeButton.onClick.AddListener(Destroy);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}