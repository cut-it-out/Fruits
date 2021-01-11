using UnityEngine;

public class Fruit : MonoBehaviour
{
    public enum Type
    {
        Apple,
        Banana,
        Peach,
        Pineapple,
        Strawberry
    }

    [SerializeField] Type fruitType;
    private Node fruitNode;

    public Type GetFruitType()
    {
        return fruitType;
    }

    public void SetFruitNode(Node n)
    {
        fruitNode = n;
    }

    public Node GetFruitNode()
    {
        return fruitNode;
    }
}
