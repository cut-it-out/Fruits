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
    [SerializeField] GameObject destroyEffect;

    public Type GetFruitType()
    {
        return fruitType;
    }

    public void AddGridPlacementToName(string newPlace)
    {
        gameObject.name = gameObject.name + " " + newPlace;
    }

    public void PlayDestroyEffect()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }
}
