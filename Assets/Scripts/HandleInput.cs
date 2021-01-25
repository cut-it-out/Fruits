using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleInput : MonoBehaviour
{
    public Event fruitClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //void Update()
    //{
    //    foreach (Touch touch in Input.touches)
    //    {
    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
    //            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
    //            if (hit.collider != null)
    //            {
    //                Debug.Log("I'm hitting START " + hit.collider.name);
    //                fruitClick.Occured(hit.collider.gameObject);
    //            }

    //        }
    //        else if (touch.phase == TouchPhase.Ended)
    //        {
    //            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
    //            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
    //            if (hit.collider != null)
    //            {
    //                Debug.Log("I'm hitting END " + hit.collider.name);
    //                fruitClick.Occured(hit.collider.gameObject);
    //            }
    //        }
    //    }
        
    //}

}
