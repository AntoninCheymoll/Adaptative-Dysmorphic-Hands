using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandComponentManager;
using static InteractableManager;
using static SystemManager;

public class HandTransformation : MonoBehaviour
{
    OVRGrabber grabber = null;
    public enum InteractionMode { normal, longer, restructure, hammer, trash, mailbox, duplicate, scissors}

    static List<HandTransformation> allHands = new List<HandTransformation>();
    HandComponentManager handComponentManager;
    
    public InteractableManager.InteractableType previousInteraction = InteractableManager.InteractableType.Null;
    public InteractionMode previousInteractionMode = InteractionMode.normal;

    Dictionary<Vector3, GameObject> associatedVectorHand = new Dictionary<Vector3, GameObject>();
    (int, int) currentTransfoInContext = (-1,-1);

    bool hand_were_duplicated = false;
    //Dictionary<InteractableManager.InteractableType, (int, int)> currentTransfoInContext = new Dictionary<InteractableManager.InteractableType, (int, int)>();

    void Start()
    {
        allHands.Add(this);
        handComponentManager = GetComponent<HandComponentManager>();
        if (grabber == null) handComponentManager.getHand(HandType.real).wrist.GetComponent<OVRGrabbable>();

        /*SystemManager.TransitionMode tm = SystemManager.getCurrentSystemTransistionMode();

        foreach (InteractableManager.InteractableType interaction in System.Enum.GetValues(typeof(InteractableManager.InteractableType) )) {
            currentTransfoInContext.Add(interaction, ((tm == SystemManager.TransitionMode.Mix)? 0: 1, 2));
        }

        currentTransfoInContext[InteractableManager.InteractableType.File] = (1, 3);
        */
    }

    void Update()
    {
        // get the current interaction state 
        InteractableType interaction = shared.getCloseInterctable(gameObject);
        InteractionMode interactMode = InteractionMode.normal;

        //Detect change of transition mode 
        if (Global.getSystemManager().didtransitionModeChanged || currentTransfoInContext.Equals((-1, -1)))
        {
            resetCurrentTransfoInContext(interaction);
        }

        // if we changed context 
        else if(interaction != previousInteraction)
        {
            resetCurrentTransfoInContext(interaction);
        }

        else if(GetComponent<GestureRecognition>().shake_detected && Global.getSystemManager().transition_mode != TransitionMode.Command)
        {
            GetComponent<GestureRecognition>().shake_Received();
            currentTransfoInContext = ((currentTransfoInContext.Item1 + 1)% currentTransfoInContext.Item2, currentTransfoInContext.Item2);
        }

        if (Global.getSystemManager().transition_mode == TransitionMode.Command)
        {
            interactMode = Global.getSystemManager().getCurrentCommandInteraction(GetComponent<HandComponentManager>().handness);

        }
        else if (currentTransfoInContext.Item1 == 0)
        {
            interactMode = InteractionMode.normal;
        }
        else if (interaction == InteractableType.Null)
        {
            interactMode = InteractionMode.normal;
        }
        else if( interaction == InteractableType.Cups)
        {
            interactMode = InteractionMode.duplicate;
        }
        else if (interaction == InteractableType.Buttons) {
            
            interactMode = InteractionMode.restructure;
        }
        else if (interaction == InteractableType.Nail) {
            
            interactMode = InteractionMode.hammer;
        }
        else if (interaction == InteractableType.Rock) {

            interactMode = InteractionMode.longer;
            
        }
        else if (interaction == InteractableType.File)
        {
            if(currentTransfoInContext.Item1 == 1)
            {
                interactMode = InteractionMode.trash;
            }
            else
            {
                interactMode = InteractionMode.mailbox;
            }
        }

        handComponentManager.activate(interactMode, interactMode != previousInteractionMode);

        if(handComponentManager.getHand(HandType.virtual_longer).go.activeSelf)
        {
            foreach (FingerTransforms ft in handComponentManager.getHand(HandType.virtual_longer).allFingers.Values)
            {
                Transform knuckle = ft.knuckles[0];
                knuckle.localScale = new Vector3(shared.Rock_FingerProportion, 1, 1);

                for (int i = 1; i < 3; i++)
                {
                    knuckle = ft.knuckles[i];
                    Transform parent = knuckle.transform.parent;
                    knuckle.transform.parent = null;
                    knuckle.localScale = new Vector3(shared.Rock_FingerProportion, 1, 1);
                    knuckle.transform.parent = parent;
                }
            }
        }

        if (handComponentManager.getHand(HandType.virtual_restructure).go.activeSelf)
        {
            HandComponents hand = handComponentManager.getHand(HandType.virtual_restructure);
            Vector3 initialHandPos = hand.wrist.position;
            Quaternion initialHandRot = hand.wrist.rotation;


            hand.wrist.position = Global.getInteractableManager().Buttons_Center.position;
            hand.wrist.rotation = (handComponentManager.handness == HandComponentManager.Handness.right) ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(180, 90, 0);


            int cpt = 1;
            foreach (FingerTransforms ft in hand.allFingers.Values)
            {

                //Find the button associated to the hand 
                int associate_button_nb = 0;
                if (ft.fingerName.Equals(Global.FingerNames.Thumb)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 1 : 5;
                if (ft.fingerName.Equals(Global.FingerNames.Index)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 2 : 4;
                if (ft.fingerName.Equals(Global.FingerNames.Middle)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 3 : 3;
                if (ft.fingerName.Equals(Global.FingerNames.Ring)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 4 : 2;
                if (ft.fingerName.Equals(Global.FingerNames.Pinky)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 5 : 1;

                GameObject associate_button = null;
                foreach (GameObject button in Global.getInteractableManager().Buttons__GOList) if (button.name == "Button (" + associate_button_nb + ")") associate_button = button;

                //Change finger orientation
                Vector3 orientationDiff_To_RestPose = GetComponent<HandComponentManager>().rest_pose[ft.fingerName] - ft.knuckles[0].localEulerAngles;
                //float y_diff_to_init = (ft.knuckles[0].localRotation.eulerAngles.y - FingerTransforms.natural_angles[ft.fingerName]) % 360;
                //if (y_diff_to_init > 180) y_diff_to_init = y_diff_to_init - 360;
                //if (y_diff_to_init < -180) y_diff_to_init = y_diff_to_init + 360;


                Quaternion initknuckleRot = ft.knuckles[0].localRotation;
                ft.knuckles[0].transform.LookAt(associate_button.transform.position, Vector3.down);
                ft.knuckles[0].transform.Rotate((handComponentManager.handness == HandComponentManager.Handness.left) ? new Vector3(0, 90, 0) : new Vector3(0, 90, 180));

                ft.knuckles[0].localEulerAngles -= orientationDiff_To_RestPose;

                //float targetY = ft.knuckles[0].localRotation.eulerAngles.y;
                //ft.knuckles[0].localRotation = initknuckleRot; 

                //ft.knuckles[0].localEulerAngles =  new Vector3(ft.knuckles[0].eulerAngles.x, targetY + y_diff_to_init, ft.knuckles[0].eulerAngles.z);

                //Change finger length

                float dist_knuckle_button = Vector3.Distance(ft.knuckles[0].position, associate_button.transform.position);
                float prop = dist_knuckle_button / Vector3.Distance(ft.tip.position, ft.knuckles[0].position);

                Transform knuckle = ft.knuckles[0];
                knuckle.localScale = new Vector3(prop, 1, 1);

                for (int i = 1; i < 3; i++)
                {
                    knuckle = ft.knuckles[i];
                    Transform parent = knuckle.transform.parent;
                    knuckle.transform.parent = null;
                    knuckle.localScale = new Vector3(prop, 1, 1);
                    knuckle.transform.parent = parent;
                }


                cpt++;
            }

            hand.wrist.position = initialHandPos;
            hand.wrist.rotation = initialHandRot;
        }

        if( handComponentManager.duplicateHandsHolder.activeSelf)
        {

            if(interactMode == InteractionMode.duplicate && interactMode != previousInteractionMode)
            {
                GameObject closest_cup = null;
                float closest_dist = float.MaxValue;

                foreach (GameObject cup in Global.getInteractableManager().Cup_GOList)
                {

                    if(Vector3.Distance(transform.position, cup.transform.position) < closest_dist)
                    {
                        closest_dist = Vector3.Distance(transform.position, cup.transform.position);
                        closest_cup = cup;
                    }
                }

                associatedVectorHand = new Dictionary<Vector3, GameObject>();
                int i = 0;
                foreach (GameObject cup in Global.getInteractableManager().Cup_GOList)
                {
                    //if (!cup.Equals(closest_cup))
                    //{
                        GameObject go = handComponentManager.duplicateHands[i];
                        go.transform.localEulerAngles = Vector3.zero;

                        associatedVectorHand.Add(cup.transform.position - closest_cup.transform.position, go);
                    //}
                    i++;
                }
            }

            foreach (KeyValuePair<Vector3, GameObject> p in associatedVectorHand)
            {

                p.Value.transform.position = transform.position + p.Key;
                p.Value.transform.rotation = transform.rotation;
            }

            hand_were_duplicated = true;

        }


        foreach (GameObject button in Global.getInteractableManager().Buttons__GOList)
        {
            button.GetComponent<ButtonBehavior>().reset();

            // check if the colliser interact with a button 
            foreach (FingerTransforms ft in handComponentManager.getHand(HandType.virtual_restructure).allFingers.Values)
            {

                if (Vector3.Distance(button.GetComponent<Collider>().bounds.ClosestPoint(ft.tip.transform.position), ft.tip.transform.position) < Global.getInteractableManager().Buttons_RangeToActivate)
                {
                    button.GetComponent<ButtonBehavior>().activate();
                }
            }
        }

        previousInteraction = interaction;
        previousInteractionMode = interactMode;
    }


    private void resetCurrentTransfoInContext(InteractableManager.InteractableType it)
    {
        switch (Global.getSystemManager().transition_mode)
        {
            case SystemManager.TransitionMode.Command:
                currentTransfoInContext = (1, 0);
                break;

            default:

                if(it == InteractableManager.InteractableType.Null) currentTransfoInContext = (1, 1);
                else if(it == InteractableManager.InteractableType.File) 
                    currentTransfoInContext = (Global.getSystemManager().transition_mode.Equals(SystemManager.TransitionMode.Mix)?0:1, 3);
                else 
                    currentTransfoInContext =(Global.getSystemManager().transition_mode.Equals(SystemManager.TransitionMode.Mix) ? 0 : 1, 2);

                break;
            
        }        
    }
}

























    





     /*
        foreach (FingerTransform ft in GetComponent<HandComponentManager>().allTransforms.allFingers.Values)
        {
            for (int i = 0; i < 3; i++)
            {
                ft.knuckles[i].localScale = new Vector3(1, 1, 1);
            }
        } 

        if(Global.currentType == Global.DysmorphicType.Longer)
        {
            foreach (FingerTransform ft in GetComponent<HandComponentManager>().allTransforms.secondaryFinger())
            {
                Transform knuckle = ft.knuckles[0];
                knuckle.localScale = new Vector3(Global.shared.Longer_proportion, 1, 1);

                for (int i = 1; i < 3; i++)
                {
                    knuckle = ft.knuckles[i];
                    Transform parent = knuckle.transform.parent;
                    knuckle.transform.parent = null;
                    knuckle.localScale = new Vector3(Global.shared.Longer_proportion, 1, 1);
                    knuckle.transform.parent = parent;
                }
            }
        }

        if (Global.currentType == Global.DysmorphicType.Outspread)
        {
            int cpt = 0;
            foreach (FingerTransform ft in GetComponent<HandComponentManager>().allTransforms.secondaryFinger())
            {
                Transform knuckle = ft.knuckles[0];
                float addition = -Global.shared.Outspead_range + cpt * (Global.shared.Outspead_range*2) / 3;
                knuckle.localEulerAngles = knuckle.localEulerAngles + new Vector3(0, addition, 0);
                cpt++;
            }
        }

        if (Global.currentType == Global.DysmorphicType.Separated)
        {
            int cpt_f = 0;
            
            foreach (string fingerName in Global.shared.fingerNames)
            {
                FingerTransform ft = GetComponent<HandComponentManager>().allTransforms.allFingers[fingerName];
                List<(Transform, Transform, Transform)> segments = new List<(Transform, Transform,Transform)> { (ft.knuckles[0], ft.knuckles[1], ft.segments[0]), 
                                                                                           (ft.knuckles[1], ft.knuckles[2], ft.segments[1]), 
                                                                                           (ft.knuckles[2], ft.tip , ft.segments[2]) };

                int cpt_k = 0;
                foreach ((Transform, Transform, Transform) segment in segments)
                {
                    float segmentSize = Vector3.Distance(segment.Item1.position, segment.Item2.position);

                    Global.changeGlobalScale(segment.Item3, new Vector3(segmentSize / 2, segmentSize / 2, segmentSize / 2));
                    
                    bool isRightHand = (GetComponent<HandComponentManager>().handness == HandComponentManager.Handness.right);
                    Vector3 v = segment.Item2.position - segment.Item1.position;
                    segment.Item3.position = Quaternion.FromToRotation(anchor.transform.eulerAngles, v).eulerAngles*100; // + Global.shared.Separated_InitPosList[cpt_f + (isRightHand?5:0)];
                    
                    //if (cpt_k == 0) segment.Item3.localRotation = segment.Item1.localRotation;
                    //else segment.Item3.localRotation = Quaternion.Euler(0, segment.Item1.localRotation.eulerAngles.x, segment.Item1.localRotation.eulerAngles.z);
                    //if (cpt_k == 0) segment.Item3.Rotate(new Vector3(0,0,90));
                    //segment.Item3.rotation = (Quaternion.FromToRotation(Vector3.up, segment.Item1.transform.position - segment.Item2.transform.position));

                    //if (cpt_k == 0) segment.Item3.rotation = segment.Item3.rotation * Quaternion.Inverse(anchor.transform.rotation); ;
                    cpt_k++;
                }

                //ft.knuckles[0].localScale = Vector3.zero;
                cpt_f++;
            }
            
        }

    }*/





