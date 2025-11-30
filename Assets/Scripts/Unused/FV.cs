using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FV : MonoBehaviour
{
    public static FV X;

    void Awake()
    {
        if (X == null)
        {
            DontDestroyOnLoad(gameObject); X = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*
        Avoid Z Value Effecting Other Vector3
    */

    public Vector3 OnlyVec2(Vector3 target, Vector3 self)
    {
        return new Vector3(target.x, target.y, self.z);
    }

    /*
        SHORTER Debugs
    */

    public void DL(string text)
    {
        #if UNITY_EDITOR
            Debug.Log(text);
        #endif
    }
    public void DLW(string text)
    {
        #if UNITY_EDITOR
            Debug.LogWarning(text);
        #endif
    }
    public void DLE(string text)
    {
        #if UNITY_EDITOR
            Debug.LogError(text);
        #endif
    }

    /*
        TO-DO:
            1. Custom Gizmos (w/ #if unity_editor)
            2. Vector2.Distance Squareli olsun.
    */
}
