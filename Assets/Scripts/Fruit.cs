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

    public Type GetFruitType()
    {
        return fruitType;
    }

}
