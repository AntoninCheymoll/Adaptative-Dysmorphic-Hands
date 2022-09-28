using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleManager : MonoBehaviour
{
    GameObject upperPart;
    GameObject toCut;
    public float distanceToCut = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Equals("UpperPart"))
            {
                upperPart = child.gameObject;

                toCut = upperPart.transform.Find("ToCut").gameObject;
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void cut(Bounds bound)
    {
        if (bound.SqrDistance(toCut.transform.position) < distanceToCut)
        {
            upperPart.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
