using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBehavior : MonoBehaviour
{
    public bool go = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            poof();
            go = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + collision.gameObject.name.Contains("Notebook"));

        if (collision.gameObject.name.Contains("Notebook"))
        {
            //go = true;
        }
    }

    public void poof()
    {
        
        if (!GetComponent<ParticleSystem>().isPlaying) {
            Debug.Log("in");
            GetComponent<ParticleSystem>().Play();
        }
    }
}
