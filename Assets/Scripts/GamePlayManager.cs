using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance;

    [SerializeField] float swapDuration = .15f;
    [SerializeField] int gameScoreBaseValue = 100;
    [SerializeField] int gameScoreExtraValue = 50;
    [SerializeField] int gameScoreIncrement = 2;

    // variables
    private GameObject selectedFruit, lastSelectedFruit;
    private Vector3 selectedPos, lastSelectedPos;
    private bool swappingInProgress = false;
    private int gameScore = 0;
    private bool isGameMenuOpen = false;
    
    // cached 
    private GameGrid gGrid;

    private void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

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
        if (!swappingInProgress && !gGrid.IsShifting && !MenuManager.GameIsPaused)
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
                        gGrid.SwapObjects(selectedFruit, lastSelectedFruit);
                        AudioManager.Instance.SwapSound();
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
            gGrid.ClearAllMatches(selectedFruit);
            gGrid.ClearAllMatches(lastSelectedFruit);
                        
            ResetVariables();
            
        }
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

    public void UpdateGameScore(int matchCount)
    {
        gameScore += matchCount * gameScoreBaseValue;

        if (matchCount > 3)
        {
            gameScore +=  (((matchCount - 3) / gameScoreIncrement) + 1) * ((matchCount - 3) * gameScoreExtraValue);
        }
        
    }

    public int GetGameScore()
    {
        return gameScore;
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }

}
