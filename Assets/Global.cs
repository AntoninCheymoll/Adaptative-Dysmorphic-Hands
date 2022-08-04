using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Global : MonoBehaviour
{
    [HideInInspector]
    public enum FingerNames {  Thumb, Index, Middle, Ring, Pinky }

    public static Global shared;

    // Start is called before the first frame update
    void Start()
    {
        shared = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space)) EditorApplication.isPaused = true;
    }

    public static void changeGlobalScale(Transform t, Vector3 scale)
    {
        Transform parent = t.parent;
        t.parent = null;
        t.localScale = scale;
        t.parent = parent;
    }


}