using System.Collections;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Components
{
    public class UILoaderView : BaseView
    {
        [Header("UI")]
        [SerializeField] private Transform loader;

        [Header("Params")]
        [SerializeField, Range(1f, 20f)] private float loaderSpeed;
        [SerializeField, Range(-360f, 360f)] private float loaderStep;

        public void LoadView()
        { }

        public void RemoveView()
        { }

        void OnEnable()
        {
            StartCoroutine(LoaderCoroutine());
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator LoaderCoroutine()
        {
            while (true)
            {
                Quaternion target = Quaternion.Euler(loader.rotation.x, loader.rotation.y, loader.rotation.eulerAngles.z + loaderStep);
                loader.rotation = target;

                yield return new WaitForSeconds(1f / loaderSpeed);
            }
        }
    }
}