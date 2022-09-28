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

    public static SystemManager getSystemManager()
    {
        return GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<SystemManager>();
    }

    public static InteractableManager getInteractableManager()
    {
        return GameObject.FindGameObjectWithTag("InteractableManager").GetComponent<InteractableManager>();
    }

    public static void toFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    public static void toOpaqueMode(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }
}