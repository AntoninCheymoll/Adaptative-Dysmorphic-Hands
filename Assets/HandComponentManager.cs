using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandComponentManager : MonoBehaviour
{
    public enum Handness {right,left }
    public enum HandType {virtuall,real}

    [Header("Name Change")]
    public bool updateCompNameB;
    public string add;

    [Header("Component")]
    Dictionary<HandType,HandComponents> bothHandsComponents = new Dictionary<HandType, HandComponents>();

    [Header("Info")]
    public Handness handness;
    public Dictionary<Global.FingerNames, Vector3> rest_pose = new Dictionary<Global.FingerNames, Vector3> { };



    // Start is called before the first frame update
    void Start()
    {
        if (updateCompNameB) updateCompName();

        bothHandsComponents.Add(HandType.real, new HandComponents(transform.Find("HandPrefab_" + ((handness == Handness.right) ? "R_" : "L_") + "real"), this));
        bothHandsComponents.Add(HandType.virtuall, new HandComponents(transform.Find("HandPrefab_" + ((handness == Handness.right) ? "R_" : "L_") + "virtual"), this));

        foreach(Global.FingerNames name in Enum.GetValues(typeof(Global.FingerNames))){
            rest_pose.Add(name, real().allFingers[name].knuckles[0].localEulerAngles);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    void updateCompName()
    {
        foreach (Component child in GetComponentsInChildren(typeof(Transform), true))
        {
            child.name = child.name + add;
        }
    }

    public HandComponents real()
    {
        return bothHandsComponents[HandComponentManager.HandType.real];
    }

    public HandComponents virtuall()
    {
        return bothHandsComponents[HandComponentManager.HandType.virtuall];
    }
}

public class HandComponents
{
    HandComponentManager hcm;

    public Transform wrist;

    public Dictionary<Global.FingerNames, FingerTransforms> allFingers = new Dictionary<Global.FingerNames, FingerTransforms>();

    public SkinnedMeshRenderer mesh;


    public HandComponents(Transform t, HandComponentManager hcm)
    {

        this.hcm = hcm;
        foreach (Component child in t.GetComponentsInChildren(typeof(Transform), true))
        {
            foreach (Global.FingerNames fingerName in Enum.GetValues(typeof(Global.FingerNames)))
            {
                if (child.name.Contains(fingerName.ToString().ToLower() + "1" )) { 
                    FingerTransforms finger = new FingerTransforms(child.transform, fingerName, hcm); 
                    allFingers.Add(fingerName, finger); }
            }

            if (child.name.Contains("wrist")) wrist = child.transform;
            if (child.GetComponent<SkinnedMeshRenderer>() != null) mesh = child.GetComponent<SkinnedMeshRenderer>();

        }
    }

    public FingerTransforms[] secondaryFinger()
    {
        return new FingerTransforms[] { index(), middle(), ring(), pinky() };
    }

    public FingerTransforms index(){ return allFingers[Global.FingerNames.Index];}
    public FingerTransforms thumb(){ return allFingers[Global.FingerNames.Thumb];}
    public FingerTransforms middle(){ return allFingers[Global.FingerNames.Middle];}
    public FingerTransforms pinky(){ return allFingers[Global.FingerNames.Pinky];}
    public FingerTransforms ring(){ return allFingers[Global.FingerNames.Ring];}

}

public class FingerTransforms
{
    //public static Dictionary<Global.FingerNames, Quaternion> natural_angles = new Dictionary<Global.FingerNames, float> { { Global.FingerNames.Thumb, -7.39f}, { Global.FingerNames.Index, 2.009f }, { Global.FingerNames.Middle, 5.947f }, { Global.FingerNames.Ring, 14.481f }, { Global.FingerNames.Pinky, -17.705f } };
    //public Dictionary<Global.FingerNames, Quaternion> rest_pose = new Dictionary<Global.FingerNames, Quaternion> {  };

    public HandComponentManager hcm;
    public Global.FingerNames fingerName;

    public Transform[] knuckles = new Transform[3];
    public Transform tip;
    
    public FingerTransforms(Transform t, Global.FingerNames fingerName, HandComponentManager hcm)
    {
        this.hcm = hcm;
        this.fingerName = fingerName;

        //rest_pose.Add(fingerName, t.localRotation);

        knuckles[0] = t;
        foreach (Component child in t.GetComponentsInChildren(typeof(Transform), true))
        {

            if (child.name.Contains("finger_tip_marker")) tip = child.transform;
            else if (child.name.Contains("2")) knuckles[1] = child.transform;
            else if (child.name.Contains("3")) knuckles[2] = child.transform;

        }

        /*for(int i = 0; i< 3; i++)
        {
            segments[i]  = GameObject.CreatePrimitive(PrimitiveType.Capsule).transform;
            segments[i].name = "Segment_" + fingerName + "_" + hcm.handness+ "_" + i;
            segments[i].localScale = Vector3.zero;
            if (i != 0) segments[i].parent = segments[i - 1];
            
        }*/

    }
}
