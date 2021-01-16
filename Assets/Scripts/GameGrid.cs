using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius = 0.5f;
    [SerializeField] Vector3 gridBottomLeftPos;
    [SerializeField] List<GameObject> fruitList;
    [SerializeField] float shiftDelay = .03f;

    GameObject[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public bool IsShifting { get; set; }

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
        grid = new GameObject[gridSizeX, gridSizeY];

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
                
                // for debug
                //fruitGameObject.GetComponent<Fruit>().AddGridPlacementToName("(" + x.ToString() + "-" + y.ToString() + ")");

                // save it to the grid
                grid[x, y] = fruitGameObject;
            }
        }        
    }

    private Vector3 GetWorldPoint(int x, int y)
    {
        return gridBottomLeftPos
                + Vector3.right * (x * nodeDiameter + nodeRadius)
                + Vector3.up * (y * nodeDiameter + nodeRadius);
    }

    public bool IsNeighbor(GameObject goOne, GameObject goTwo)
    {
        GetGameObjectGridPos(goOne, out int nodeOneX, out int nodeOneY);
        GetGameObjectGridPos(goTwo, out int nodeTwoX, out int nodeTwoY);
        
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


    public GameObject GetGameObjectFromWorldPoint(Vector3 worldPosition)
    {
        GetGameObjectGridPos(worldPosition, out int x, out int y);

        return grid[x,y];
    }

    public void GetGameObjectGridPos(GameObject go, out int x, out int y)
    {
        GetGameObjectGridPos(go.transform.position, out x, out y);
    }

    public void GetGameObjectGridPos(Vector3 worldPosition, out int x, out int y)
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

    public void RemoveObjectFromGrid(GameObject go)
    {
        GetGameObjectGridPos(go, out int goX, out int goY);
        grid[goX, goY] = null;
    }

    public void SwapObjects(GameObject goOne, GameObject goTwo)
    {
        GetGameObjectGridPos(goOne, out int goOneX, out int goOneY);
        GetGameObjectGridPos(goTwo, out int goTwoX, out int goTwoY);
        grid[goOneX, goOneY] = goTwo;
        grid[goTwoX, goTwoY] = goOne;
    }

    public void PrintGridPos(GameObject go)
    {
        GetGameObjectGridPos(go, out int goX, out int goY);
        Debug.Log("GRID(x: " + goX + " y: " + goY + ")");
        if (grid[goX,goY]==null)
        {
            Debug.Log("GRID entry is NULL");
        }
        else
        {
            Debug.Log(grid[goX, goY].GetComponent<Fruit>().GetFruitType());
        }
        Debug.Log("***********");
    }

    public IEnumerator FindEmptyTiles()
    //public void FindEmptyTiles()
    {
        Debug.Log("FindEmptyTiles invoke in gridgame");
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y] == null)
                {
                    Debug.Log("ShiftTilesDown(x, y); x: " + x + " y: " + y);
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    //ShiftTilesDown(x, y);
                    break;
                }
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart)
    //private void ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        IsShifting = true;
        int nullCount = 0;
        
        for (int y = yStart; y < gridSizeY; y++)
        {
            if (grid[x, y] == null)
            {
                nullCount++;
                Debug.Log("looking for null; x: " + x + " y: " + y);
                Debug.Log("nullCount: " + nullCount);
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i <= gridSizeY - yStart; i++)
        {
            if (yStart + i + nullCount >= gridSizeY || grid[x, yStart + i + nullCount] == null)
            {
                break;
            }
            yield return new WaitForSeconds(shiftDelay);
            
            Debug.Log("start+i " + yStart + i);
            Debug.Log("start+nullCount " + yStart + i + nullCount );
            grid[x, yStart + i + nullCount].transform.position = GetWorldPoint(x, yStart + i);
            grid[x, yStart + i] = grid[x, yStart + i + nullCount];
            grid[x, yStart + i + nullCount] = null;
            
            Debug.Log("-------");
        }
        IsShifting = false;
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    
                    if (grid[x, y] == null)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        switch (grid[x, y].GetComponent<Fruit>().GetFruitType())
                        {
                            case Fruit.Type.Banana:
                                Gizmos.color = Color.black;
                                break;
                            case Fruit.Type.Apple:
                                Gizmos.color = Color.green;
                                break;
                            case Fruit.Type.Peach:
                                Gizmos.color = Color.yellow;
                                break;
                            case Fruit.Type.Pineapple:
                                Gizmos.color = Color.blue;
                                break;
                            case Fruit.Type.Strawberry:
                                Gizmos.color = Color.red;
                                break;
                            default:
                                Gizmos.color = Color.cyan;
                                break;
                        }
                    }

                    Vector3 gizmoPos = grid[x, y] ? grid[x, y].transform.position : GetWorldPoint(x, y);

                    Gizmos.DrawCube(
                        gizmoPos,
                        new Vector3(nodeDiameter - 0.7f, nodeDiameter - 0.7f, 1f)
                        );
                }
            }
        }
    }
}
