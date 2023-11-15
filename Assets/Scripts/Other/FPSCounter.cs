using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour
    {
        const float FpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string Display = "{0} FPS";
        private Text m_Text;


        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + FpsMeasurePeriod;
            m_Text = GetComponent<Text>();
        }


        private void Update()
        {
            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/FpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += FpsMeasurePeriod;
                if (m_CurrentFps > 50)
                {
                    m_Text.color = Color.green;
                }
                else if (m_CurrentFps > 25)
                {
                    m_Text.color = Color.white;
                }
                else 
                {
                    m_Text.color = Color.red;
                }
                m_Text.text = string.Format(Display, m_CurrentFps);
            }
        }
    }
}
