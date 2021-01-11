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
            SwapTwoFruits();
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

    private void SwapTwoFruits()
    {
        Vector3 tempPos = selectedFruit.transform.position;

        selectedFruit.transform.position = Vector3.Lerp(selectedFruit.transform.position, lastSelectedPos, swapDuration);
        lastSelectedFruit.transform.position = Vector3.Lerp(lastSelectedFruit.transform.position, selectedPos, swapDuration);

        if (selectedFruit.transform.position == lastSelectedPos && lastSelectedFruit.transform.position == selectedPos)
        {
            ResetSelectionsAndVariables();
        }
    }

    private void ResetSelectionsAndVariables()
    {
        SetSelection(selectedFruit, false);
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

}
