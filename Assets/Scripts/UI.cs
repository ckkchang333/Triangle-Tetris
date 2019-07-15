﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    [Header("Main Menu")]
    public GameObject mainMenuText;
    public GameObject authorText;
    public int mainMenuIndex = 0;
    public int mainMenuMaxIndex = 3;
    public float displace;

    [Header("Controls")]
    public GameObject controlsInfoBox;
    private bool controlsVisible = false;

    [Header("Pause Menu")]
    public GameObject pauseText;
    public int pauseMaxIndex = 1;
    private int pauseIndex = 0;

    [Header("Post Game Menu")]
    public GameObject highScoreText;
    public GameObject newHighScoreText;
    public GameObject gameOverText;


    [Header("Settings Box")]
    public GameObject settingsBoard;

    [Header("Other")]
    public GameObject scoreText;
    public GameObject selectorSprite;

    public bool active;
    public int uiIndex;

    private Vector3 startingSelectorPosition;


    [Header("External")]
    public GameObject gameBoard;


    public void toggleActive(bool toggle, int currentUiIndex)
    {
        active = toggle;
        uiIndex = currentUiIndex;
    }

    private void rotateSelector(bool clockwise)
    {
        if(clockwise)
        {
            selectorSprite.transform.Rotate(new Vector3(0, 0, -90));
        }
        else
        {
            selectorSprite.transform.Rotate(new Vector3(0, 0, 90));
        }
    }

    public void endGameUI(bool newHighScoreFlag, int newHighScore)
    {
        gameOverText.SetActive(true);
        if(newHighScoreFlag)
        {
            newHighScoreText.SetActive(true);
            //Debug.Log(newHighScore);
            highScoreText.GetComponent<Text>().text = "High Score: " + newHighScore;
            highScoreText.SetActive(true);
        }
        selectorSprite.transform.position = startingSelectorPosition;
        selectorSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
        uiIndex = 0;
    }

    public void resetGameUI()
    {
        pauseText.SetActive(false);
        selectorSprite.SetActive(false);
        uiIndex = -1;
        scoreText.GetComponent<Text>().text = "Rows Cleared: 0";
    }


    public void replayTitle()
    {
        
        toggleActive(false, 0);
        gameBoard.GetComponent<Board>().replayTitle();

    }

	// Use this for initialization
	void Start () {
        startingSelectorPosition = selectorSprite.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(active)
        {
            
            // Game Paused
            if (uiIndex == -2)
            {
                pauseText.SetActive(true);
                selectorSprite.SetActive(true);
                if(!controlsVisible)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow) && pauseIndex > 0)
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, displace));
                        selectorSprite.transform.position += new Vector3(0, displace);
                        pauseIndex = --pauseIndex;

                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow) && pauseIndex < pauseMaxIndex)
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, -displace));
                        selectorSprite.transform.position -= new Vector3(0, displace);
                        pauseIndex = ++pauseIndex;
                    }
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        rotateSelector(true);
                    }
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        rotateSelector(false);
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (pauseIndex == 0)
                        {
                            resetGameUI();
                            //gameBoard.GetComponent<Board>().resetGame();
                            gameBoard.GetComponent<Board>().startGame();
                            uiIndex = -1;
                        }
                        else if (pauseIndex == 1)
                        {
                            controlsVisible = true;
                            controlsInfoBox.SetActive(true);
                        }
                        else if (pauseIndex == 2)
                        {
                            settingsBoard.SetActive(true);
                            uiIndex += 3;
                        }
                        else if (pauseIndex == 3)
                        {
                            Application.Quit();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Q))
                    {
                        pauseText.SetActive(false);
                        selectorSprite.SetActive(false);
                        uiIndex = -1;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {

                    controlsVisible = false;
                    controlsInfoBox.SetActive(false);
                }
            }
            // Playing the Game
            else if (uiIndex == -1)
            {
                if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Q))
                {
                    selectorSprite.transform.position = startingSelectorPosition;
                    selectorSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                    pauseIndex = 0;
                    uiIndex = -2;
                }
            }
            else if (uiIndex == 0)
            {
                if(!controlsVisible)
                {
                    if(Input.GetKeyDown(KeyCode.F10))
                    {
                        replayTitle();
                    }
                    mainMenuText.SetActive(true);
                    selectorSprite.SetActive(true);
                    authorText.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        rotateSelector(true);
                    }
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        rotateSelector(false);
                    }
                    if (Input.GetKeyDown(KeyCode.UpArrow) && mainMenuIndex > 0)
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, displace));
                        selectorSprite.transform.position += new Vector3(0, displace);
                        mainMenuIndex = --mainMenuIndex;

                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow) && mainMenuIndex < mainMenuMaxIndex)
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, -displace));
                        selectorSprite.transform.position -= new Vector3(0, displace);
                        mainMenuIndex = ++mainMenuIndex;
                    }
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if(mainMenuIndex == 0)
                    {
                        mainMenuText.SetActive(false);
                        selectorSprite.SetActive(false);
                        gameOverText.SetActive(false);
                        newHighScoreText.SetActive(false);
                        authorText.SetActive(false);
                        scoreText.GetComponent<Text>().text = "Rows Cleared: 0";
                        gameBoard.GetComponent<Board>().startGame();
                        scoreText.SetActive(true);
                        uiIndex = -1;
                    }
                    else if (mainMenuIndex == 1)
                    {
                        controlsVisible = true;
                        controlsInfoBox.SetActive(true);
                    }
                    else if (mainMenuIndex == 2)
                    {
                        uiIndex += 3;
                    }
                    else if (mainMenuIndex == 3)
                    {
                        //Debug.Log("Quitting");
                        Application.Quit();
                    }
                }
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    controlsVisible = false;
                    controlsInfoBox.SetActive(false);
                }
            }
            // Main Menu Settings
            else if(uiIndex >= 1)
            {
                settingsBoard.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    settingsBoard.GetComponent<SettingsMenu>().updateGameBoardDasAndArr();
                    settingsBoard.GetComponent<SettingsMenu>().resestSelectorPosition();
                    uiIndex -= 3;
                    settingsBoard.SetActive(false);
                }
            }
        }
        else
        {
            mainMenuText.SetActive(false);
            selectorSprite.SetActive(false);
            authorText.SetActive(false);
            gameOverText.SetActive(false);
            scoreText.SetActive(false);
            newHighScoreText.SetActive(false);
        }
    }
}
