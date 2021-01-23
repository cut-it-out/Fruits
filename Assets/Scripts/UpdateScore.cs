using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour
{
    [SerializeField] Text scoreText = null;
    
    void Update()
    {
        scoreText.text = GamePlayManager.Instance.GetGameScore().ToString();
    }
}
