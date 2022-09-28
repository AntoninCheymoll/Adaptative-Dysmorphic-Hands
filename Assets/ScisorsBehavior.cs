using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScisorsBehavior : MonoBehaviour
{
    GameObject Surrogate_Index;
    GameObject Surrogate_Index_Tip;
    GameObject Surrogate_Index_Blade;
    GameObject Surrogate_Middle;
    GameObject Surrogate_Middle_Tip;
    GameObject Surrogate_Center;

    HandComponents associate_hand;
    GameObject hand_Center;
    GameObject hand_Middle_Tip;
    GameObject hand_Index_Tip;

    public float cutDetectedDistance = 0.01f;
    bool uncut = true;

    // Start is called before the first frame update
    void Start()
    {
        
        Surrogate_Index = transform.FindChildRecursive("Index").gameObject;
        Surrogate_Middle = transform.FindChildRecursive("Middle").gameObject;
        Surrogate_Center = transform.FindChildRecursive("Center").gameObject;

        Surrogate_Index_Tip = Surrogate_Index.transform.FindChildRecursive("Tip").gameObject;
        Surrogate_Index_Blade = Surrogate_Index.transform.FindChildRecursive("Blade").gameObject;
        Surrogate_Middle_Tip = Surrogate_Middle.transform.FindChildRecursive("Tip").gameObject;

        associate_hand = transform.parent.GetComponent<HandComponentManager>().getHand(HandComponentManager.HandType.real);

        hand_Center = associate_hand.go.transform.FindChildRecursive("center_scissors").gameObject;
        hand_Middle_Tip = associate_hand.go.transform.FindChildRecursive("middle_scissors").gameObject;
        hand_Index_Tip = associate_hand.go.transform.FindChildRecursive("index_scissors").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        bool hr = associate_hand.hcm.handness == HandComponentManager.Handness.right;
        //Index.transform.RotateAround(Center.transform.position, Center.transform.forward, 1);

        Surrogate_Index.transform.up = (-hand_Index_Tip.transform.position + hand_Center.transform.position).normalized;// + (Index.transform.up.normalized - (Index_Tip.transform.position - Center.transform.position).normalized).normalized;
        Surrogate_Middle.transform.up = (-hand_Middle_Tip.transform.position + hand_Center.transform.position).normalized;// + (Index.transform.up.normalized - (Index_Tip.transform.position - Center.transform.position).normalized).normalized;


        Vector3 planeNormal_Index = Surrogate_Index.transform.up;
        Vector3 projectionA_Index = Vector3.ProjectOnPlane(-Surrogate_Index.transform.right, planeNormal_Index);
        Vector3 projectionB_Index = Vector3.ProjectOnPlane(( hr?(-hand_Index_Tip.transform.position + hand_Middle_Tip.transform.position): (hand_Index_Tip.transform.position - hand_Middle_Tip.transform.position)).normalized, planeNormal_Index);
        float angle_Index = Vector3.Angle(projectionA_Index, projectionB_Index);

        Surrogate_Index.transform.Rotate(-Surrogate_Index.transform.up, (hr?0:180) + angle_Index, Space.World); // (hand_Index_Tip.transform.position - hand_Center.transform.position).normalized;// + (Index.transform.up.normalized - (Index_Tip.transform.position - Center.transform.position).normalized).normalized;

        Vector3 planeNormal_Middle = Surrogate_Middle.transform.up;
        Vector3 projectionA_Middle = Vector3.ProjectOnPlane(Surrogate_Middle.transform.right, planeNormal_Middle);
        Vector3 projectionB_Middle = Vector3.ProjectOnPlane((hr ? (hand_Middle_Tip.transform.position - hand_Index_Tip.transform.position) : (-hand_Middle_Tip.transform.position + hand_Index_Tip.transform.position)).normalized, planeNormal_Middle);
        float angle_Middle = Vector3.Angle(projectionA_Middle, projectionB_Middle);

        Surrogate_Middle.transform.Rotate(Surrogate_Middle.transform.up, (hr ? 180 : 0)+angle_Middle, Space.World);
        Surrogate_Center.transform.rotation = Surrogate_Middle.transform.rotation;

        transform.position = hand_Center.transform.position - Surrogate_Center.transform.position + transform.position;





        if (Vector3.Distance(Surrogate_Index_Tip.transform.position, Surrogate_Middle_Tip.transform.position) < cutDetectedDistance)
        {
            if (uncut)
            {
                foreach(GameObject pole in Global.getInteractableManager().Pole_GOList)
                {
                    pole.GetComponent<PoleManager>().cut(Surrogate_Index_Blade.GetComponent<MeshRenderer>().bounds);
                }
                uncut = false;
            }
        }
        else
        {
            uncut = true;
        }
         
        
    }
}
