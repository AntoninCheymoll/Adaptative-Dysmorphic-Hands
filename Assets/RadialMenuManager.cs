using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static HandComponentManager;
using static HandTransformation;
using static SystemManager;

public class RadialMenuManager : MonoBehaviour
{
    GameObject radial_menu;
    public GameObject text_mesh;
    float radialRay = 3.4f;

    List<GameObject> text_meshs = new List<GameObject>();
    bool radial_was_active = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Component child in GetComponentsInChildren(typeof(Transform), true))
        {        
            if (child.name.Equals("RadialMenu")) radial_menu = child.gameObject;
        }

        LookAtConstraint constraint = radial_menu.transform.GetComponent<LookAtConstraint>();
        ConstraintSource cs = new ConstraintSource();
        cs.sourceTransform = Camera.main.transform;
        constraint.AddSource(cs);

        int n = 0;
        List<InteractionMode> list = new List<InteractionMode>();

        foreach (InteractionMode mode in Enum.GetValues(typeof(InteractionMode))) {
            if (!(GetComponent<HandComponentManager>().handness == Handness.left) || (mode != InteractionMode.trash && mode != InteractionMode.mailbox)) { 
                list.Add(mode);
            }
        }

        //Cmd_InteractionType[] tititi = Enum.GetValues(typeof(Cmd_InteractionType));
        //List<Cmd_InteractionType> list = Enum.GetValues(typeof(Cmd_InteractionType));
        //.AddRange(Enum.GetValues(typeof(Cmd_InteractionType)));

        //new List<Cmd_InteractionType>(Enum.GetValues(typeof(Cmd_InteractionType)));

        foreach (InteractionMode interactor in Enum.GetValues(typeof(InteractionMode)))
        {
            GameObject text = Instantiate(text_mesh);
            text.GetComponent<TextMesh>().text = interactor.ToString();
            
            text.transform.SetParent(radial_menu.transform);
            float degre = (float)Math.PI * 2 *  n / Enum.GetValues(typeof(InteractableManager.InteractableType)).Length + (float)Math.PI/2 ;
            text.transform.localPosition = new Vector3((radialRay / 2) * (float)Math.Cos(degre), (radialRay *3 / 4) * (float)Math.Sin(degre), 0);
            
            LookAtConstraint constraint_in = text.transform.GetComponent<LookAtConstraint>();
            ConstraintSource cs_in = new ConstraintSource();
            cs_in.sourceTransform = Camera.main.transform;
            constraint_in.AddSource(cs);

            text_meshs.Add(text);
            n++;
        }

        radial_menu.transform.SetParent(null);

    }

    // Update is called once per frame
    void Update()
    {
        if (Global.getSystemManager().transition_mode == SystemManager.TransitionMode.Command)
        {
            
            bool is_active = GetComponent<GestureRecognition>().fist_detected;

            foreach (GameObject go in text_meshs)
            {
                float fist_text_distance = go.GetComponent<MeshCollider>().bounds.SqrDistance(GetComponent<HandComponentManager>().getHand(HandComponentManager.HandType.real).palm_center.position);
                if (fist_text_distance < 0.01)
                {
                    go.GetComponent<TextMesh>().color = new Color(1, 0, 0);
                    if (radial_was_active && !is_active)
                    {
                        foreach (InteractionMode interactor in Enum.GetValues(typeof(InteractionMode)))
                        {
                            if (interactor.ToString().Equals(go.GetComponent<TextMesh>().text))
                            { 
                                Global.getSystemManager().SetCurrentCommandInteraction(interactor, GetComponent<HandComponentManager>().handness); 
                            }
                        }

                    }
                    //Debug.Log();
                }
                else
                {
                    go.GetComponent<TextMesh>().color = new Color(0, 0, 0);
                }
            }

            radial_menu.SetActive(is_active);
            Vector3 hand_position = GetComponent<HandComponentManager>().getHand(HandType.real).palm_center.transform.position;
            if (!radial_menu.activeSelf) radial_menu.transform.position = hand_position + (hand_position - Camera.main.transform.position).normalized * 0.20f;


            radial_was_active = radial_menu.activeSelf;
        }
        
    }
}
