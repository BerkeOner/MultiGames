using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject); Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
