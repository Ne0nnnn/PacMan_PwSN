﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static bool isOnePlayerGame = true;

    public Text playerText1;
    public Text exit;
    public Text playerSelector;

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (!isOnePlayerGame)
            {
                isOnePlayerGame = true;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText1.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isOnePlayerGame)
            {
                isOnePlayerGame = false;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, exit.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            if (isOnePlayerGame)
            {
                isOnePlayerGame = true;
                SceneManager.LoadScene("LevelMenu");
                Debug.Log("zaladowano menu");
            }
            else if (!isOnePlayerGame)
            {
                isOnePlayerGame = false;
                doExitGame();
                Debug.Log("Wychodzenie z gry");
            }
        }
    }

    void doExitGame()
    {
        Application.Quit();
    }
}
