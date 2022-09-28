using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandTransformation;

public class HandComponentManager : MonoBehaviour
{
    public enum Handness {right,left }
    public enum HandType {real, virtual_normal, virtual_longer, virtual_restructure}

    [Header("Name Change")]
    public bool updateCompNameB;
    public string add;

    [Header("Component")]
    Dictionary<HandType,HandComponents> hands = new Dictionary<HandType, HandComponents>();
    List<GameObject> allGO;

    [HideInInspector]
    public GameObject hammer;
    [HideInInspector]
    public GameObject trash;
    [HideInInspector]
    public GameObject mailbox;
    [HideInInspector]
    public GameObject graspRock;
    [HideInInspector]
    public GameObject duplicateHandsHolder;
    [HideInInspector]
    public List<GameObject> duplicateHands = new List<GameObject>();
    [HideInInspector]
    public GameObject scissors;


    [Header("Info")]
    public Handness handness;
    public Dictionary<Global.FingerNames, Vector3> rest_pose = new Dictionary<Global.FingerNames, Vector3> { };
    public Dictionary<InteractionMode, List<GameObject>> associated_interMode_GO = new Dictionary<InteractionMode, List<GameObject>>();

    Dictionary<InteractionMode, float> isDisapearing = new Dictionary<InteractionMode, float>();
    float last_changed = 0;

    // Start is called before the first frame update
    void Awake()
    {

        if (updateCompNameB) updateCompName();

        foreach (Transform child in transform)
        {
            
            if (child.name.Contains("Hammer")) hammer = child.gameObject;
            if (child.name.Contains("HandGraspsRock")) graspRock = child.gameObject;
            if (child.name.Contains("Trash")) trash = child.gameObject;
            if (child.name.Contains("Mailbox")) mailbox = child.gameObject;
            if (child.name.Contains("DuplicateHandsHolder")) duplicateHandsHolder = child.gameObject;
            if (child.name.Contains("Scissors")) scissors = child.gameObject;
        }

        Debug.Log(scissors);

        InteractableManager inter_manager =  Global.getInteractableManager().GetComponent<InteractableManager>();
        foreach(GameObject cups in inter_manager.Cup_GOList)
        {

            GameObject dup_hand = Instantiate(handness == Handness.right ? inter_manager.Cup_RightHandPrefab : inter_manager.Cup_LeftHandPrefab);
            dup_hand.name = "Duplicated Hand";
            duplicateHands.Add(dup_hand);
            dup_hand.transform.parent = duplicateHandsHolder.transform;
            
        }


        foreach (HandType handType in Enum.GetValues(typeof(HandType)))
        {
            hands.Add(handType, new HandComponents(transform.Find("HandPrefab_" + ((handness == Handness.right) ? "R_" : "L_") + handType.ToString()), this));
        }

        foreach(Global.FingerNames name in Enum.GetValues(typeof(Global.FingerNames))){
            rest_pose.Add(name, getHand(HandType.real).allFingers[name].knuckles[0].localEulerAngles);
        }

        allGO = new List<GameObject> { hammer, graspRock, hands[HandType.virtual_normal].go, hands[HandType.virtual_longer].go, hands[HandType.virtual_restructure].go, scissors };
        allGO.AddRange(duplicateHands);
        
        if(handness == Handness.left)
        {
            allGO.Add(mailbox);
            allGO.Add(trash);
        }

        associated_interMode_GO.Add(InteractionMode.duplicate, duplicateHands);
        associated_interMode_GO.Add(InteractionMode.hammer, new List<GameObject>() { hammer });
        associated_interMode_GO.Add(InteractionMode.longer, new List<GameObject>() { getHand(HandType.virtual_longer).go});
        associated_interMode_GO.Add(InteractionMode.normal, new List<GameObject>() { getHand(HandType.virtual_normal).go});
        associated_interMode_GO.Add(InteractionMode.restructure, new List<GameObject>() { getHand(HandType.virtual_restructure).go});
        associated_interMode_GO.Add(InteractionMode.trash, new List<GameObject>() { trash });
        associated_interMode_GO.Add(InteractionMode.mailbox, new List<GameObject>() { mailbox });
        associated_interMode_GO.Add(InteractionMode.scissors, new List<GameObject>() { scissors });
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

    public HandComponents getHand(HandType ht)
    {
        return hands[ht];
    }

    public void activate(InteractionMode interactionMode, bool interactionModeJustChanged)
    {
        List<GameObject> toKeep = new List<GameObject>() ;

        if (Global.getSystemManager().morphic_mode == SystemManager.MorphingMode.Gradual)
        {
            foreach ((InteractionMode, float) pair in updateAndGetIsDisapearing(interactionMode, interactionModeJustChanged))
            {
                List<GameObject> associated_GO = associated_interMode_GO[pair.Item1];
                toKeep.AddRange(associated_GO);

                foreach (GameObject go in associated_GO)
                {
                    List<Material> allMaterials = new List<Material>();

                    getAllMaterials(go, allMaterials);

                    foreach (Material material in allMaterials)
                    {
                        if(pair.Item2 < 1)Global.toFadeMode(material);
                        else Global.toOpaqueMode(material);

                        Color c = material.color;
                        material.color = new Color(c.r,c.g,c.b, pair.Item2);
                    }
                    /*
                    foreach (SkinnedMeshRenderer skinnedmesh in all_skinnedmeshes)
                    {
                        if (pair.Item2 != 0) Global.toFadeMode(skinnedmesh.materials[0]);
                        else Global.toOpaqueMode(skinnedmesh.materials[0]);

                        Color c = skinnedmesh.materials[0].color;
                        skinnedmesh.materials[0].color = new Color(c.r, c.g, c.b, 1 - pair.Item2);
                    }*/

                }                        
            }
        }
        else
        {
            toKeep.AddRange(associated_interMode_GO[interactionMode]);
        }

        foreach (GameObject go in allGO)
        {
            go.SetActive(toKeep.Contains(go));

        }
    }

    private void getAllMaterials(GameObject go, List<Material> materials)
    {
        //if (handness == Handness.left) Debug.Log(go);
        if (go.gameObject.GetComponent<MeshRenderer>() != null) materials.AddRange(go.gameObject.GetComponent<MeshRenderer>().materials);
        if (go.gameObject.GetComponent<SkinnedMeshRenderer>() != null) materials.AddRange(go.gameObject.GetComponent<SkinnedMeshRenderer>().materials);

        foreach (Transform t in go.transform)
        {
            getAllMaterials(t.gameObject, materials);
        }
    }


    List<(InteractionMode, float)> updateAndGetIsDisapearing(InteractionMode current_interactionMode, bool interactionModeJustChanged)
    {
        if(interactionModeJustChanged) last_changed = Time.time;
        isDisapearing[current_interactionMode] = Time.time;
        
        List<InteractionMode> toRemove = new List<InteractionMode>();
        List<(InteractionMode, float)> ret = new List<(InteractionMode, float)>();

        foreach (KeyValuePair<InteractionMode, float> pair in isDisapearing)
        {
            if (Time.time - pair.Value > Global.getSystemManager().morphing_gradual_duration) toRemove.Add(pair.Key);
            if(current_interactionMode == pair.Key)
            {
                ret.Add( (pair.Key, (Time.time - last_changed) / Global.getSystemManager().morphing_gradual_duration));
                //Debug.Log((Time.time - pair.Value) + " " + interactionModeJustChanged);
            }
            else
            {
                Debug.Log( 1- (Time.time - pair.Value) / Global.getSystemManager().morphing_gradual_duration);
                ret.Add((pair.Key,  1- (Time.time - pair.Value) / Global.getSystemManager().morphing_gradual_duration));
            }
        }

        foreach (InteractionMode inter in toRemove) isDisapearing.Remove(inter);
        return ret;
    }
}


public class HandComponents
{
    public HandComponentManager hcm;

    public GameObject go;
    public Transform wrist;
    public Transform palm_center;

    public Dictionary<Global.FingerNames, FingerTransforms> allFingers = new Dictionary<Global.FingerNames, FingerTransforms>();

    public SkinnedMeshRenderer mesh;


    public HandComponents(Transform t, HandComponentManager hcm)
    {
        go = t.gameObject;
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
            if (child.name.Contains("palm_center_marker")) palm_center = child.transform;
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
