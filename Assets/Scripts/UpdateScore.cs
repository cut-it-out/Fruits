using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour
{
    GamePlayManager gp;
    [SerializeField] Text scoreText = null;

    // Start is called before the first frame update
    void Start()
    {
        gp = FindObjectOfType<GamePlayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = gp.GetGameScore().ToString();
    }
}
