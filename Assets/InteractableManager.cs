using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager shared;

    public enum InteractableType { Null, Rock, Buttons, Cups, Nail, File, Pole}
    //List<(GameObject, InteractableType)> cancelled = new List<(GameObject, InteractableType)>();

    public List<(List<GameObject>, GameObject, float)> interactable_objs;

    [Header("General")]
    public GameObject ActivationArea;
    [Header("Buttons")]
    public float Buttons_transfoRange;
    public List<GameObject> Buttons__GOList = new List<GameObject>();
    public Transform Buttons_Center;
    public float Buttons_RangeToActivate;
    [Header("Rock")]
    public float Rock_transfoRange;
    public GameObject Rock_GO;
    public float Rock_FingerProportion;
    [Header("Cups")]
    public float Cup_transfoRange;
    public List<GameObject> Cup_GOList;
    public GameObject Cup_LeftHandPrefab;
    public GameObject Cup_RightHandPrefab;
    [Header("Nail")]
    public float Nail_transfoRange;
    public GameObject Nail_GO;
    [Header("File")]
    public float File_transfoRange;
    public GameObject File_GO;
    [Header("Poles")]
    public float Poles_transfoRange;
    public List<GameObject> Pole_GOList;


    // Start is called before the first frame update
    void Start()
    {
        shared = this;

        interactable_objs = new List<(List<GameObject>, GameObject, float)>() {
                (new List<GameObject>() {Rock_GO}, Instantiate(ActivationArea), Rock_transfoRange),
                (Buttons__GOList, Instantiate(ActivationArea), Buttons_transfoRange),
                (Cup_GOList, Instantiate(ActivationArea), Cup_transfoRange),
                (new List<GameObject>() { Nail_GO }, Instantiate(ActivationArea), Nail_transfoRange),
                (new List<GameObject>() { File_GO }, Instantiate(ActivationArea), File_transfoRange),
                (Pole_GOList, Instantiate(ActivationArea), Poles_transfoRange) };
        
    }

// Update is called once per frame
    void Update()
    {
        foreach ((List<GameObject>, GameObject, float) obj in interactable_objs)
        {
            obj.Item2.transform.position = getAvgPos(obj.Item1);
            obj.Item2.transform.localScale = new Vector3(obj.Item3 * 2, obj.Item3 * 2, obj.Item3 * 2);
        }
    }


    /*public void cancelTransformation(GameObject hand)
    {
        InteractableType currentType = getCloseInterctable(hand);

        bool flag = true;
        for (int i = cancelled.Count - 1; i >= 0; i--)
        {
            (GameObject, InteractableType) couple = cancelled[i];
            if (couple.Item1.Equals(hand) && !couple.Item2.Equals(currentType))
            {
                flag = false;
                cancelled.RemoveAt(i);
            }
        }

        if (flag && !currentType.Equals(InteractableType.Null)) cancelled.Add((hand, currentType));
    }*/

    public InteractableType getCloseInterctable(GameObject hand)
    {

        Vector3 handPos = hand.transform.position; 

        InteractableType ret = InteractableType.Null;
        float min_dist = float.MaxValue;

        List<(Vector3, float, InteractableType)> triples = new List<(Vector3, float, InteractableType)> {
            (Rock_GO.transform.position, Rock_transfoRange, InteractableType.Rock),
            (Nail_GO.transform.position, Nail_transfoRange, InteractableType.Nail),
            (getAvgPos(Buttons__GOList), Buttons_transfoRange, InteractableType.Buttons) ,
            (getAvgPos(Cup_GOList), Cup_transfoRange, InteractableType.Cups),
            (getAvgPos(Pole_GOList), Poles_transfoRange, InteractableType.Pole)
        };

        if (hand.GetComponent<HandComponentManager>().handness == HandComponentManager.Handness.left)
        {
            triples.Add((File_GO.transform.position, File_transfoRange, InteractableType.File));
        }

        foreach ((Vector3, float, InteractableType) triple in triples)
        {

            if (Vector3.Distance(handPos, triple.Item1) < triple.Item2)
            {
                if ((Vector3.Distance(handPos, triple.Item1) - triple.Item2) < min_dist){
                    ret = triple.Item3;
                    min_dist = Vector3.Distance(handPos, triple.Item1) - triple.Item2;
                }
            }
        }

        
        /*for(int i= cancelled.Count-1; i>=0; i--)
        {
            (GameObject, InteractableType) couple = cancelled[i];
            if (couple.Item1.Equals(hand) && !couple.Item2.Equals(ret))
            {
                cancelled.RemoveAt(i);
            }
        }

        foreach ((GameObject, InteractableType) couple in cancelled)
        {

            if (couple.Item1.Equals(hand) && couple.Item2.Equals(ret))
            {
                return InteractableType.Null;
            }
        }*/

        return ret;
    }

    Vector3 getAvgPos(List<GameObject> list)
    {
        Vector3 res = Vector3.zero;
        list.ForEach(x => res += x.transform.position);
        return res / list.Count;
    }
}


