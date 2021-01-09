using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 worldPosition { get; }
    public Fruit.Type fruit { get; } 
    
    public Node(Fruit.Type _fruit, Vector3 _worldPos)
    {
        fruit = _fruit;
        worldPosition = _worldPos;
    }

}
