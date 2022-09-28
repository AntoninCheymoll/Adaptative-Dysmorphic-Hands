using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPose : MonoBehaviour
{
    OVRGrabber grabber = null ;
    List<GameObject> pose_go_list = new List<GameObject>();
    GameObject grabbed = null;
    [HideInInspector]
    public bool meshShouldntBeActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.name.Contains("HandGrasps")) pose_go_list.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
       /* meshShouldntBeActivated = false;

        if (grabber == null) grabber = GetComponent<HandComponentManager>().real().wrist.GetComponent<OVRGrabber>();

        foreach (GameObject grab_obj in pose_go_list)
        { 
            if(grabber.grabbedObject != null && grabber.grabbedObject.name.Contains(grab_obj.name.Replace("HandGrasps", ""))){

                if (grab_obj.name.Equals("HandGraspsRock") && !GetComponent<HandTransformation>().previousInteraction.Equals(InteractableManager.InteractableType.Rock) )
                {
                    grab_obj.SetActive(false);
                }
                else
                {
                    grab_obj.SetActive(true);
                    GetComponent<HandComponentManager>().virtuall().mesh.enabled = false;
                    meshShouldntBeActivated = true;
                    grabber.grabbedObject.GetComponent<MeshRenderer>().enabled = false;
                    grabbed = grabber.grabbedObject.gameObject;
                }
            }
            else
            {
                grab_obj.SetActive(false);
            }
        }

        if (grabber.grabbedObject == null)
        {
            if(grabbed != null)
            {

                grabbed.GetComponent<MeshRenderer>().enabled = true;
                grabbed = null;
            }
            
        }
       */
    }
}
