using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailBehavior : MonoBehaviour
{
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("Hammer") && (Time.time > timer + 0.5f))
        {
            transform.position -= new Vector3(0,0.01f,0);
            timer = Time.time;  
        }
    }
}
