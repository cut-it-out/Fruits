using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GameObject fruitGameObject { get; }
    public Fruit.Type fruit { get; } 
    
    public Node(Fruit.Type _fruit, GameObject _fruitGameObject)
    {
        fruit = _fruit;
        fruitGameObject = _fruitGameObject;
    }

    public Vector3 GetPosision()
    {
        return fruitGameObject.transform.position;
    }

}
