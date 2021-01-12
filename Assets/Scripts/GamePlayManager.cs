using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] float swapDuration = .15f;

    // variables
    private GameObject selectedFruit, lastSelectedFruit;
    private Vector3 selectedPos, lastSelectedPos;
    private bool swappingInProgress = false;
    private bool matchFound = false;

    // cached 
    private GameGrid gGrid;

    private void Start()
    {
        gGrid = GetComponent<GameGrid>();
    }

    private void Update()
    {
        if (swappingInProgress)
        {
            HandleFruitSwap();
        }
    }

    public void HandleFruitSelected(GameObject justClickedFruit)
    {
        if (!swappingInProgress)
        {
            if (justClickedFruit == selectedFruit)
            {
                SetSelection(selectedFruit, false);
                selectedFruit = null;
            }
            else
            {
                if (!selectedFruit)
                {
                    selectedFruit = justClickedFruit;
                    SetSelection(selectedFruit, true);
                }
                else
                {
                    // check if new selection is a neighbor
                    if (gGrid.IsNeighbor(selectedFruit, justClickedFruit))
                    {
                        lastSelectedFruit = selectedFruit;
                        selectedFruit = justClickedFruit;
                        SetSelection(selectedFruit, true);

                        selectedPos = selectedFruit.transform.position;
                        lastSelectedPos = lastSelectedFruit.transform.position;

                        // trigger swapping
                        swappingInProgress = true;
                    }
                    else
                    {
                        SetSelection(selectedFruit, false);
                        selectedFruit = justClickedFruit;
                        SetSelection(selectedFruit, true);
                    }

                }
            }
        }
    }

    private void HandleFruitSwap()
    {
        Vector3 tempPos = selectedFruit.transform.position;

        selectedFruit.transform.position = Vector3.Lerp(selectedFruit.transform.position, lastSelectedPos, swapDuration);
        lastSelectedFruit.transform.position = Vector3.Lerp(lastSelectedFruit.transform.position, selectedPos, swapDuration);

        if (selectedFruit.transform.position == lastSelectedPos && lastSelectedFruit.transform.position == selectedPos)
        {
            //Debug.Log("selected>");
            ClearAllMatches(selectedFruit);
            //Debug.Log("lastSelected>");
            ClearAllMatches(lastSelectedFruit);
                        
            ResetVariables();
        }
    }

    private void ClearAllMatches(GameObject go)
    {
        //Debug.Log("ClearAllMatches - name - " + go.name);
        //Debug.Log("ClearAllMatches - pos - " + go.transform.position);
        FindAndClearMatch(go, new Vector2[2] { Vector2.left, Vector2.right });
        FindAndClearMatch(go, new Vector2[2] { Vector2.up, Vector2.down });

        if (matchFound)
        {
            Destroy(go);
        }
        matchFound = false;
    }

    private void ResetVariables()
    {
        if(selectedFruit)
            SetSelection(selectedFruit, false);
        if(lastSelectedFruit)
            SetSelection(lastSelectedFruit, false);
        selectedFruit = null;
        lastSelectedFruit = null;
        selectedPos = Vector3.zero;
        lastSelectedPos = Vector3.zero;
        
        swappingInProgress = false;
    }

    private void SetSelection(GameObject go, bool isActive)
    {
        go.transform.Find("Selection").gameObject.SetActive(isActive);
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
                Destroy(matchingFruits[i]);
            }
            matchFound = true;
        }

    }


    private List<GameObject> FindMatchInDirection(GameObject go, Vector2 castDir)
    {
        List<GameObject> matchingFruits = new List<GameObject>();

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
        return matchingFruits;

    }
}
