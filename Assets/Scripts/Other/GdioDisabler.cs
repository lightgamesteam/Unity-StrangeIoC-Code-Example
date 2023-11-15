using UnityEngine;

public class GdioDisabler : MonoBehaviour
{
    void Awake()
    {
#if !DEVELOP
        gameObject.SetActive(false);
#endif

    }
}
