using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    GameObject blue_button;
    GameObject red_button;

    //bool hasBeenTouched = false;

    // Start is called before the first frame update
    void Start()
    {
        blue_button = gameObject.transform.Find("ButtonPushBlue").gameObject;
        red_button = gameObject.transform.Find("ButtonPushRed").gameObject;
        blue_button.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate()
    {

        blue_button.SetActive(true);
        red_button.SetActive(false);
    }

    public void reset()
    {

        blue_button.SetActive(false);
        red_button.SetActive(true);
        
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Hand")){

            collNb++;
        
            blue_button.SetActive(true);
            red_button.SetActive(false);

        }
    }


    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag.Equals("Hand")) collNb--;


        if (collNb <= 0)
        {
            blue_button.SetActive(false);
            red_button.SetActive(true);
        }
        
    }
    */
}
