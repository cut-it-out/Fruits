using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius = 0.5f;
    [SerializeField] Vector3 gridBottomLeftPos;
    [SerializeField] List<GameObject> fruitList;

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Start()
    {
        Initialize();
        CreateGrid();
    }

    public void Initialize()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint =
                    gridBottomLeftPos
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.up * (y * nodeDiameter + nodeRadius);

                // randomly select a prefab from the list
                GameObject prefab = fruitList[UnityEngine.Random.Range(0, fruitList.Count)];

                // Instantiate fruit
                GameObject fruitGameObject = Instantiate(prefab, worldPoint, Quaternion.identity);
                Fruit fruit = fruitGameObject.GetComponent<Fruit>();
                
                // create Node for new fruit
                Node fruitNode = new Node(fruit.GetFruitType(), fruitGameObject);

                // save it to the grid
                grid[x, y] = fruitNode;
                // add it to the gameObject
                fruit.SetFruitNode(fruitNode);

            }
        }        
    }

    public bool IsNeighbor(Node nodeOne, Node nodeTwo)
    {
        GetNodePositionFromWorldPoint(nodeOne.GetPosision(), out int nodeOneX, out int nodeOneY);
        GetNodePositionFromWorldPoint(nodeTwo.GetPosision(), out int nodeTwoX, out int nodeTwoY);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                // exclude diagonals - only check horizontal and vertical
                if (x == -1 && y == -1)
                    continue;
                if (x == -1 && y == 1)
                    continue;
                if (x == 1 && y == -1)
                    continue;
                if (x == 1 && y == 1)
                    continue;

                int checkX = nodeOneX + x;
                int checkY = nodeOneY + y;

                // check if we are still in the grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    // check for match
                    if (checkX == nodeTwoX && checkY == nodeTwoY)
                    {
                        return true;
                    }
                }
            }
        }

        return false;

    }


    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        GetNodePositionFromWorldPoint(worldPosition, out int x, out int y);

        return grid[x,y];
    }

    public void GetNodePositionFromWorldPoint(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.Clamp(
                Mathf.RoundToInt(Mathf.Clamp(worldPosition.x - nodeRadius, 0, gridWorldSize.x) / nodeDiameter),
                0,
                gridSizeX - 1);

        y = Mathf.Clamp(
                Mathf.RoundToInt(Mathf.Clamp(worldPosition.y - nodeRadius, 0, gridWorldSize.y) / nodeDiameter),
                0,
                gridSizeY - 1);
    }


    //void OnDrawGizmos()
    //{
    //    if (grid != null)
    //    {
    //        foreach (Node n in grid)
    //        {

    //            switch (n.fruit)
    //            {
    //                case Fruit.Type.Banana:
    //                    Gizmos.color = Color.yellow;
    //                    break;
    //                case Fruit.Type.Apple:
    //                    Gizmos.color = Color.green;
    //                    break;
    //                case Fruit.Type.Peach:
    //                    Gizmos.color = Color.cyan;
    //                    break;
    //                case Fruit.Type.Pineapple:
    //                    Gizmos.color = Color.blue;
    //                    break;
    //                case Fruit.Type.Strawberry:
    //                    Gizmos.color = Color.red;
    //                    break;
    //                default:
    //                    Gizmos.color = Color.gray;
    //                    break;
    //            }

    //            Gizmos.DrawCube(
    //                n.worldPosition,
    //                new Vector3(nodeDiameter, nodeDiameter, 1f)
    //                );
    //        }
    //    }
    //}
}
