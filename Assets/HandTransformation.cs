using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandTransformation : MonoBehaviour
{
    static List<HandTransformation> allHands = new List<HandTransformation>();
    HandComponentManager handComponentManager;
    public InteractableManager.InteractableType previousInteraction = InteractableManager.InteractableType.Null;

    Dictionary<Vector3, GameObject> associatedVectorHand = new Dictionary<Vector3, GameObject>();

    void Start()
    {
        allHands.Add(this);
        handComponentManager = GetComponent<HandComponentManager>();
    }

    void Update()
    {   
        InteractableManager.InteractableType interaction = InteractableManager.shared.getCloseInterctable(gameObject);
        
        float rock_augment_ratio = interaction == InteractableManager.InteractableType.Rock? InteractableManager.shared.Rock_FingerProportion: 1;
        
        foreach (FingerTransforms ft in GetComponent<HandComponentManager>().virtuall().allFingers.Values)
        {
            Transform knuckle = ft.knuckles[0];
            knuckle.localScale = new Vector3(rock_augment_ratio, 1, 1);

            for (int i = 1; i < 3; i++)
            {
                knuckle = ft.knuckles[i];
                Transform parent = knuckle.transform.parent;
                knuckle.transform.parent = null;
                knuckle.localScale = new Vector3(rock_augment_ratio, 1, 1);
                knuckle.transform.parent = parent;
            }
        }

        if (interaction == InteractableManager.InteractableType.Buttons)
        {


            Vector3 initialHandPos = handComponentManager.virtuall().wrist.position;
            Quaternion initialHandRot = handComponentManager.virtuall().wrist.rotation;


            handComponentManager.virtuall().wrist.position = InteractableManager.shared.Buttons_Center.position;
            handComponentManager.virtuall().wrist.rotation = (handComponentManager.handness == HandComponentManager.Handness.right) ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(180, 90, 0);


            int cpt = 1;
            foreach (FingerTransforms ft in GetComponent<HandComponentManager>().virtuall().allFingers.Values)
            {

                //Find the button associated to the hand 
                int associate_button_nb = 0;
                if (ft.fingerName.Equals(Global.FingerNames.Thumb)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 1 : 5;
                if (ft.fingerName.Equals(Global.FingerNames.Index)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 2 : 4;
                if (ft.fingerName.Equals(Global.FingerNames.Middle)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 3 : 3;
                if (ft.fingerName.Equals(Global.FingerNames.Ring)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 4 : 2;
                if (ft.fingerName.Equals(Global.FingerNames.Pinky)) associate_button_nb = (handComponentManager.handness == HandComponentManager.Handness.right) ? 5 : 1;

                GameObject associate_button = null;
                foreach (GameObject button in InteractableManager.shared.Buttons__GOList) if (button.name == "Button (" + associate_button_nb + ")") associate_button = button;

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

            handComponentManager.virtuall().wrist.position = initialHandPos;
            handComponentManager.virtuall().wrist.rotation = initialHandRot;

            
        }

        if(interaction == InteractableManager.InteractableType.Cups)
        {
            if(!interaction.Equals(previousInteraction))
            {
                GameObject closest_cup = null;
                float closest_dist = float.MaxValue;

                foreach (GameObject cup in InteractableManager.shared.Cup_GOList)
                {

                    if(Vector3.Distance(transform.position, cup.transform.position) < closest_dist)
                    {
                        closest_dist = Vector3.Distance(transform.position, cup.transform.position);
                        closest_cup = cup;
                    }
                }

                associatedVectorHand = new Dictionary<Vector3, GameObject>();
                foreach (GameObject cup in InteractableManager.shared.Cup_GOList)
                {
                    if (!cup.Equals(closest_cup))
                    {
                        GameObject go = Instantiate(handComponentManager.handness == HandComponentManager.Handness.right ? InteractableManager.shared.Cup_RightHandPrefab : InteractableManager.shared.Cup_LeftHandPrefab);
                        go.transform.parent = transform.parent.parent;
                        go.transform.localEulerAngles = Vector3.zero;

                        associatedVectorHand.Add(cup.transform.position - closest_cup.transform.position, go);
                    }
                    
                }
            }

            foreach (KeyValuePair<Vector3, GameObject> p in associatedVectorHand)
            {

                p.Value.transform.position = transform.position + p.Key;
                p.Value.transform.rotation = transform.rotation;
            }

        }
        else
        {
            foreach (KeyValuePair<Vector3, GameObject> p in associatedVectorHand)
            {
                Destroy(p.Value);
            }
        }

        foreach (GameObject button in InteractableManager.shared.Buttons__GOList)
        {
            button.GetComponent<ButtonBehavior>().reset();

            // check if the colliser interact with a button 
            foreach (FingerTransforms ft in GetComponent<HandComponentManager>().virtuall().allFingers.Values)
            {

                if (Vector3.Distance(button.GetComponent<Collider>().bounds.ClosestPoint(ft.tip.transform.position), ft.tip.transform.position) < InteractableManager.shared.Buttons_RangeToActivate)
                {
                    button.GetComponent<ButtonBehavior>().activate();
                }
            }
        }

        previousInteraction = interaction;
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





