using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius = 0.5f;
    [SerializeField] Vector3 gridBottomLeftPos;
    [SerializeField] List<GameObject> fruitList;
    [SerializeField] float shiftDelay = .03f;

    GameObject[,] grid;
    Dictionary<Fruit.Type, GameObject> fruits = new Dictionary<Fruit.Type, GameObject>();
    List<Fruit.Type> allFruitList;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    private bool matchFound = false;

    public bool IsShifting { get; set; }

    private void Start()
    {
        Initialize();
        FillUpGrid();
    }

    public void Initialize()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        // init grid
        grid = new GameObject[gridSizeX, gridSizeY];

        // init fruit dictionary
        foreach (GameObject fruitObject in fruitList)
        {
            Fruit.Type ft = fruitObject.GetComponent<Fruit>().GetFruitType();
            if (fruits.ContainsKey(ft))
            {
                Debug.LogWarning("Fruit list contains more then one from " + ft.ToString() + ". Second entry was not added to the game.");
            }
            else
            {
                fruits.Add(ft, fruitObject);
            }
        }

        // init all fruit list
        allFruitList = Enum.GetValues(typeof(Fruit.Type)).Cast<Fruit.Type>().ToList();
    }

    private void FillUpGrid()
    {
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (!grid[x, y])
                {
                    AddNewFruitToGrid(x, y);
                }
            }
        }        
    }

    private void AddNewFruitToGrid(int x, int y)
    {
        Vector3 worldPoint =
            gridBottomLeftPos
            + Vector3.right * (x * nodeDiameter + nodeRadius)
            + Vector3.up * (y * nodeDiameter + nodeRadius);

        // Instantiate fruit
        GameObject fruitGameObject = Instantiate(GetNewFruit(x, y), worldPoint, Quaternion.identity);

        // for debug
        //fruitGameObject.GetComponent<Fruit>().AddGridPlacementToName("(" + x.ToString() + "-" + y.ToString() + ")");

        // save it to the grid
        grid[x, y] = fruitGameObject;
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
        Debug.Log("***** item details ****");
        Debug.Log("GRID(x: " + goX + " y: " + goY + ")");
        if (grid[goX,goY]==null)
        {
            Debug.Log("GRID entry is NULL");
        }
        else
        {
            Debug.Log(grid[goX, goY].GetComponent<Fruit>().GetFruitType());
        }
        Debug.Log("***********************");
    }

    public void ClearAllMatches(GameObject go, bool test = false)
    {
        if (test)
        {
            FindAndClearMatch(go, new Vector2[1] { Vector2.right });
            FindAndClearMatch(go, new Vector2[1] { Vector2.up });

        }
        else
        {

            FindAndClearMatch(go, new Vector2[2] { Vector2.left, Vector2.right });
            FindAndClearMatch(go, new Vector2[2] { Vector2.up, Vector2.down });
        }

        if (matchFound)
        {
            go.GetComponent<Fruit>().PlayDestroyEffect();
            Destroy(go);
            RemoveObjectFromGrid(go);
            StopCoroutine(FindEmptyTiles());
            StartCoroutine(FindEmptyTiles());
        }
        matchFound = false;
    }

    private void FindAndClearMatch(GameObject go, Vector2[] directions)
    {
        List<GameObject> matchingFruits = new List<GameObject>();

        for (int i = 0; i < directions.Length; i++)
        {
            matchingFruits.AddRange(FindMatchInDirection(go, directions[i]));
        }

        if (matchingFruits.Count >= 2)
        {
            for (int i = 0; i < matchingFruits.Count; i++)
            {
                matchingFruits[i].GetComponent<Fruit>().PlayDestroyEffect();
                Destroy(matchingFruits[i]);
                RemoveObjectFromGrid(matchingFruits[i]);

            }
            matchFound = true;
        }

    }

    private List<GameObject> FindMatchInDirection(GameObject go, Vector2 castDir)
    {
        List<GameObject> matchingFruits = new List<GameObject>();

        if (go)
        {
            go.GetComponent<BoxCollider2D>().enabled = false; // disable collider so would not hit itself
            RaycastHit2D hit = Physics2D.Raycast(go.transform.position, castDir);
            go.GetComponent<BoxCollider2D>().enabled = true; // enable collider so next time it will work :)

            while (hit.collider != null && hit.collider.GetComponent<Fruit>().GetFruitType() == go.GetComponent<Fruit>().GetFruitType())
            {
                matchingFruits.Add(hit.collider.gameObject);
                BoxCollider2D boxCollider = hit.collider.gameObject.GetComponent<BoxCollider2D>();
                boxCollider.enabled = false; // disable collider so would not hit itself
                hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
                boxCollider.enabled = true; // enable collider so next time it will work :)
            }
        }

        return matchingFruits;

    }

    public IEnumerator FindEmptyTiles()
    {
        //Debug.Log("FindEmptyTiles invoke in gridgame");
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y] == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int gridX = 0; gridX < gridSizeX; gridX++)
        {
            for (int gridY = 0; gridY < gridSizeY; gridY++)
            {
                ClearAllMatches(grid[gridX, gridY]);
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart)
    {
        IsShifting = true;
        int nullCount = 0;
        
        for (int y = yStart; y < gridSizeY; y++)
        {
            if (grid[x, y] == null)
            {
                nullCount++;
            }
            else
            {
                break;
            }
        }
        
        for (int moveTo = yStart; moveTo < gridSizeY; moveTo++)
        {
            int moveFrom = moveTo + nullCount;

            //Debug.Log("moveFrom " + moveFrom + " --- moveTo " + moveTo);

            yield return new WaitForSeconds(shiftDelay);

            if (moveFrom >= gridSizeY)
            {
                //Debug.Log("moveFrom >= gridSizeY");
                if (grid[x, moveTo] == null)
                {
                    //Debug.Log("grid[x, moveTo] == null");
                    AddNewFruitToGrid(x, moveTo);
                }
            }
            else
            {
                if (!grid[x, moveFrom])
                {

                    //Debug.Log("grid[x, moveFrom] is NULL --> x: " + x + " y: " + moveFrom);
                    continue;
                }
                
                grid[x, moveFrom].transform.position = GetWorldPoint(x, moveTo);
                grid[x, moveTo] = grid[x, moveFrom];
                grid[x, moveFrom] = null;
            }

        }
        IsShifting = false;        
    }

    private GameObject GetNewFruit(int x, int y)
    {
        //Debug.Log("New Fruit for > x: " + x + " y: " + y);
        List<Fruit.Type> possibleFruits = new List<Fruit.Type>();
        possibleFruits.AddRange(allFruitList);

        if (x > 0)
        {
            possibleFruits.Remove(grid[x - 1, y].GetComponent<Fruit>().GetFruitType());
        }

        if (x < gridSizeX - 1 && grid[x + 1, y])
        {
            possibleFruits.Remove(grid[x + 1, y].GetComponent<Fruit>().GetFruitType());
        }

        if (y > 0)
        {
            possibleFruits.Remove(grid[x, y - 1].GetComponent<Fruit>().GetFruitType());
        }

        Fruit.Type fruitType = RandomFruit(possibleFruits);
        
        return fruits[fruitType];
    }

    public Fruit.Type RandomFruit(List<Fruit.Type> fruitList = null)
    {
        // if nothing to exclude then return random from enum
        if (fruitList == null)
        {
            return (Fruit.Type)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Fruit.Type)).Length);
        }
        // there are fruits which needs to be excluded
        else
        {
            return fruitList[UnityEngine.Random.Range(0, fruitList.Count)];
        }
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
