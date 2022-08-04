using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextManager : MonoBehaviour
{
    static Text text;
    // Start is called before the first frame update
    
    void Start()
    {
        text = GetComponent<Text>();    
    }

    // Update is called once per frame
    public static void setText(string s)
    {
        text.text = s;
    }
}
