using UnityEngine;

namespace PFS.Assets.Scripts.Views.Homeworks
{
    public class UIHomeworksView : BaseView
    {
        public void LoadView()
        {
            SetScreenColliderSize();
        }

        public void RemoveView()
        {

        }

        private void SetScreenColliderSize()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            if (collider)
            {
                collider.size = GetComponent<RectTransform>()?.sizeDelta ?? Vector2.zero;
            }
        }
    }
}