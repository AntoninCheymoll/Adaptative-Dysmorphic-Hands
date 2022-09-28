using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandComponentManager;

public class GestureRecognition : MonoBehaviour
{
    [Header("General")]

    int timeCpt = 0;

    [Header("Shake")]

    GameObject shake_MiddleKnuckle;

    Vector3 shake_previousPosition = Vector3.zero;
    Vector3 shake_previousAcceleration = Vector3.zero;

    readonly int shake_DetectedThreshold = 2;
    readonly int shake_TimeCptRange = 20;
    readonly float shake_magnitudeThreshold=0.01f;
    List<int> shake_DetectedTimePoints = new List<int>();
    
    readonly int shake_ignoreFrames = 20;
    int shake_remaingingFrameToIgnore = 0;

    public bool shake_detected = false;
    public bool fist_detected = false;
     

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeCpt++;

        if (shake_detected) return;



        if (shake_remaingingFrameToIgnore == 0)
        {

            if (shake_DetectedTimePoints.Count > 0 && shake_DetectedTimePoints[0] < timeCpt - shake_TimeCptRange)
            {
                shake_DetectedTimePoints.RemoveAt(0);
            }

            if (shake_MiddleKnuckle == null) shake_MiddleKnuckle = GetComponent<HandComponentManager>().getHand(HandType.real).middle().tip.gameObject;

            Vector3 currentPosition = shake_MiddleKnuckle.transform.position;
            if (shake_previousAcceleration != null)
            {

                Vector3 current_acceleration = currentPosition - shake_previousPosition;

                bool detected = Vector3.Dot(current_acceleration, shake_previousAcceleration) < 0 && current_acceleration.magnitude > shake_magnitudeThreshold;
                if (detected) shake_DetectedTimePoints.Add(timeCpt);

            }

            if (shake_previousPosition == null)
            {
                shake_previousPosition = currentPosition;
            }
            else
            {
                shake_previousAcceleration = currentPosition - shake_previousPosition;
                shake_previousPosition = currentPosition;

            }

            if (shake_DetectedTimePoints.Count > shake_DetectedThreshold)
            {
                //InteractableManager.shared.cancelTransformation(gameObject);
                //shake_DetectedTimePoints = new List<int>();
                shake_detected = true;
            }
            else
            {

            }

        }
        else shake_remaingingFrameToIgnore--;


    }

    public void shake_Received()
    {
        shake_remaingingFrameToIgnore = shake_ignoreFrames;
        shake_detected = false;
    }
}
