using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleClick : MonoBehaviour
{
    public Event fruitClick;

    void OnMouseDown()
    {
        //Debug.Log("clicked: " + name);
        fruitClick.Occured(gameObject);
    }

}
