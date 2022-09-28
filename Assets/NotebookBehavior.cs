using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookBehavior : MonoBehaviour
{

    Vector3 initialPosition;
    // Start is called before the first frame update
    
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("Trash") || collision.collider.name.Contains("Mailbox"))
        {
            
            if(!collision.gameObject.GetComponent<ParticleSystem>().isPlaying) collision.gameObject.GetComponent<ParticleSystem>().Play();
            transform.position = initialPosition;
            
            

        }
    }
}
