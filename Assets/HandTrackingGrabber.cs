using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandTrackingGrabber : OVRGrabber
{

    bool prev_pinch = false;
    new protected void CheckForGrabOrRelease(float prevFlex)
    {
        bool curr_pinch = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Hands);

        if (curr_pinch && !prev_pinch)
        {
            GrabBegin();
        }
        else if (!curr_pinch && prev_pinch)
        {
            GrabEnd();
        }

        prev_pinch = curr_pinch;
    }

}
