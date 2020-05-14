using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    [Header("Main Menu")]
    public GameObject mainMenuText;
    public GameObject authorText;
    public int mainMenuIndex = 0;
    public int mainMenuIndexMax = 3;
    public float mainMenuDisplace;


    [Header("Play Menu")]
    public GameObject playMenuText;
    public int playMenuIndex = 0;
    public int playMenuIndexMax = 3;
    public float playMenuDisplace;
    public Text gameTimer;
    public List<string> gameModeDescriptions;
    public Text gameModeDescriptionText;
    public GameObject creditsBoard;
    private bool displayingCredits;

    [Header("Controls")]
    public GameObject controlsInfoBox;
    private bool controlsVisible = false;
    

    [Header("Pause Menu")]
    public GameObject pauseText;
    public int pauseMaxIndex = 1;
    private int pauseIndex = 0;

    [Header("Post Game Menu")]
    //public GameObject highScoreText;
    public GameObject newHighScoreText;
    public GameObject gameOverText;


    [Header("Settings Box")]
    public GameObject settingsBoard;

    [Header("Other")]
    public GameObject scoreText;
    public GameObject selectorSprite;
    public Text marathonLevelText;

    public GameObject highscoreRows;
    public GameObject highscoreTime;

    public bool active;
    public int uiIndex;
    

    public SettingsManager settings;


    [Header("External")]
    public GameObject gameBoard;


    private KeyCode rotateLeftKey = KeyCode.Z;
    private KeyCode rotateRightKey = KeyCode.X;
    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;
    private KeyCode replayTitleKey = KeyCode.F10;
    private KeyCode pauseKey = KeyCode.P;
    private int rotateLeftIndex = 0;
    private int rotateRightIndex = 1;
    private int upKeyIndex = 0;
    private int downKeyIndex = 1;
    private int replayTitleIndex = 10;
    private int pauseKeyIndex = 9;

    public AudioSource audioSource;
    public AudioClip slideRotateClip;
    public AudioClip newMenuClip;

    public Vector3 mainMenuStartPosition;
    public Vector3 playMenuStartPosition;
    private Vector3 playMenuStartPositionBase;
    public Vector3 pauseMenuStartPosition;

    public void setGameTimer(float timePassed)
    {
        gameTimer.text = ((int)timePassed / 60).ToString() + ":" + (timePassed % 60 < 10 ? "0" : "") + (Mathf.Round(timePassed % 60 * 100) / 100.0f).ToString() + (Mathf.Round(timePassed % 60 * 100) % 100 != 0 ? ((Mathf.Round(timePassed % 60 * 100) % 100 == 0 ? "0" : "") + (Mathf.Round(timePassed % 60 * 100) % 10 == 0 ? "0" : "")) : ".00");
        //if(timePassed < 60)
        //{
        //    gameTimer.text = timePassed.ToString();
        //}
        //else
        //{
        //    gameTimer.text = ((int) timePassed / 60).ToString() + ":" + (Mathf.Round(timePassed % 60 / 0.001f) / 1000.0).ToString();
        //}
    }

    public void updateMarathonLevelText(int newLevel)
    {
        marathonLevelText.text = "Level " + newLevel;
    }

    public int getUiIndex()
    {
        return uiIndex;
    }

    public void setKeys(List<KeyCode> newKeys)
    {
        rotateLeftKey = newKeys[rotateLeftIndex];
        rotateRightKey = newKeys[rotateRightIndex];
        replayTitleKey = newKeys[replayTitleIndex];
        pauseKey = newKeys[pauseKeyIndex];

    }


    public void toggleActive(bool toggle, int currentUiIndex)
    {
        active = toggle;
        uiIndex = currentUiIndex;
    }

    private void rotateSelector(bool clockwise)
    {
        audioSource.PlayOneShot(slideRotateClip);
        if (clockwise)
        {
            selectorSprite.transform.Rotate(new Vector3(0, 0, -90));
        }
        else
        {
            selectorSprite.transform.Rotate(new Vector3(0, 0, 90));
        }
    }

    //public void endGameUI(int gameMode, bool newHighScoreFlag, int newHighScore)
    //{
    //    gameOverText.SetActive(true);
    //    if(newHighScoreFlag)
    //    {
    //        newHighScoreText.SetActive(true);
    //        //Debug.Log(newHighScore);
    //        highScoreText.GetComponent<Text>().text = "High Score: " + newHighScore;
    //        highScoreText.SetActive(true);
    //    }
    //    //selectorSprite.transform.position = startingSelectorPosition;
    //    selectorSprite.transform.localPosition = mainMenuStartPosition;
    //    selectorSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    uiIndex = 0;
    //}
    public void endGameUI(int gameMode, bool newHighScoreFlag, int rowsCleared = 0, float gameTime = 0)
    {
        Debug.Log("Ending Game");
        Debug.Log(gameMode);
        Debug.Log(newHighScoreFlag);
        Debug.Log(rowsCleared);
        Debug.Log(gameTime);
        gameOverText.SetActive(true);
        if (newHighScoreFlag)
        {
            newHighScoreText.SetActive(true);
            if(gameMode == 1)
            {
                playMenuText.transform.Find("SPRINT SCORE").GetComponent<Text>().text = "R: " + rowsCleared + " / T: " + ((int)gameTime / 60).ToString() + ":" + (gameTime % 60 < 10 ? "0" : "") + (Mathf.Round(gameTime % 60 * 100) / 100.0f).ToString() + (Mathf.Round(gameTime % 60 * 100) % 100 != 0 ? ((Mathf.Round(gameTime % 60 * 100) % 100 == 0 ? "0" : "") + (Mathf.Round(gameTime % 60 * 100) % 10 == 0 ? "0" : "")) : ".00");
                Debug.Log(playMenuText.transform.Find("SPRINT SCORE").GetComponent<Text>().text);
                playMenuText.transform.Find("SPRINT SCORE").gameObject.SetActive(true);
            }
            else if (gameMode == 2)
            {
                playMenuText.transform.Find("MARATHON SCORE").GetComponent<Text>().text = "R: " + rowsCleared + " / T: " + ((int)gameTime / 60).ToString() + ":" + (gameTime % 60 < 10 ? "0" : "") + (Mathf.Round(gameTime % 60 * 100) / 100.0f).ToString() + (Mathf.Round(gameTime % 60 * 100) % 100 != 0 ? ((Mathf.Round(gameTime % 60 * 100) % 100 == 0 ? "0" : "") + (Mathf.Round(gameTime % 60 * 100) % 10 == 0 ? "0" : "")) : ".00");
                Debug.Log(playMenuText.transform.Find("MARATHON SCORE").GetComponent<Text>().text);
                playMenuText.transform.Find("MARATHON SCORE").gameObject.SetActive(true);
            }
            //Debug.Log(newHighScore);
            //highScoreText.GetComponent<Text>().text = "High Score: " + newHighScore;
            //highScoreText.SetActive(true);
        }
        marathonLevelText.gameObject.SetActive(false);
        //selectorSprite.transform.position = startingSelectorPosition;
        selectorSprite.transform.localPosition = mainMenuStartPosition;
        selectorSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
        uiIndex = 0;
        mainMenuIndex = 0;
        //playMenuIndex = 0;
    }

    public void resetGameUI()
    {
        pauseText.SetActive(false);
        selectorSprite.SetActive(false);
        uiIndex = -1;
        scoreText.GetComponent<Text>().text = "Rows Cleared: 0";

        if (playMenuIndex == 3)
        {
            updateMarathonLevelText(0);
        }
    }


    public void replayTitle()
    {
        
        toggleActive(false, 0);
        gameBoard.GetComponent<Board>().replayTitle();

    }

	// Use this for initialization
	void Start () {
        //startingSelectorPosition = selectorSprite.transform.position;
        selectorSprite.transform.localPosition = mainMenuStartPosition;
        playMenuStartPositionBase = playMenuStartPosition;
    }
	
	// Update is called once per frame
	void Update () {
		if(active)
        {

            // Game Paused      pause Menu
            if (uiIndex == -2)
            {
                pauseText.SetActive(true);
                //selectorSprite.SetActive(true);
                if (!controlsVisible)
                {
                    //if (Input.GetKeyDown(KeyCode.UpArrow) && pauseIndex > 0)
                    //{
                    //    //selectorSprite.transform.Translate(new Vector3(0, displace));
                    //    selectorSprite.transform.position += new Vector3(0, mainMenuDisplace);
                    //    pauseIndex = --pauseIndex;
                    //    audioSource.PlayOneShot(slideRotateClip);

                    //}
                    //if (Input.GetKeyDown(KeyCode.DownArrow) && pauseIndex < pauseMaxIndex)
                    //{
                    //    //selectorSprite.transform.Translate(new Vector3(0, -displace));
                    //    selectorSprite.transform.position -= new Vector3(0, mainMenuDisplace);
                    //    pauseIndex = ++pauseIndex;
                    //    audioSource.PlayOneShot(slideRotateClip);
                    //}
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, displace));
                        pauseIndex = --pauseIndex;
                        audioSource.PlayOneShot(slideRotateClip);

                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, -displace));
                        pauseIndex = ++pauseIndex;
                        audioSource.PlayOneShot(slideRotateClip);
                    }
                    if(pauseIndex < 0)
                    {
                        pauseIndex = pauseMaxIndex;
                    }
                    else if(pauseIndex > pauseMaxIndex)
                    {
                        pauseIndex = 0;
                    }
                    selectorSprite.transform.localPosition = pauseMenuStartPosition - new Vector3(0, pauseIndex * mainMenuDisplace);
                    if (Input.GetKeyDown(rotateRightKey))
                    {
                        rotateSelector(true);
                        audioSource.PlayOneShot(slideRotateClip);
                    }
                    if (Input.GetKeyDown(rotateLeftKey))
                    {
                        rotateSelector(false);
                        audioSource.PlayOneShot(slideRotateClip);
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (pauseIndex == 0)
                        {
                            gameBoard.GetComponent<Board>().pause();
                            pauseText.SetActive(false);
                            selectorSprite.SetActive(false);
                            uiIndex = -1;
                        }
                        else if (pauseIndex == 1)
                        {
                            uiIndex = 0;
                            pauseText.SetActive(false);
                            selectorSprite.transform.localPosition = mainMenuStartPosition;
                            gameBoard.GetComponent<Board>().pieceQueue.SetActive(false);
                            gameBoard.GetComponent<Board>().pieceHolder.SetActive(false);
                            gameTimer.gameObject.SetActive(false);
                            marathonLevelText.gameObject.SetActive(false);
                            scoreText.SetActive(false);
                            newHighScoreText.SetActive(false);
                            highscoreRows.SetActive(false);
                            highscoreTime.SetActive(false);
                            //highScoreText.SetActive(false);
                            audioSource.PlayOneShot(newMenuClip, 0.1f);
                        }
                        //else if (pauseIndex == 1)
                        //{
                        //    controlsVisible = true;
                        //    controlsInfoBox.SetActive(true);
                        //}
                        //else if (pauseIndex == 2)
                        //{
                        //    settingsBoard.SetActive(true);
                        //    uiIndex += 3;
                        //}
                        //else if (pauseIndex == 3)
                        //{
                        //    Application.Quit();
                        //}
                        else if (pauseIndex == 2)
                        {
                            settingsBoard.SetActive(true);
                            uiIndex += 4;
                            audioSource.PlayOneShot(newMenuClip, 0.1f);
                        }
                        else if (pauseIndex == 3)
                        {
                            Application.Quit();
                        }
                    }
                    if (Input.GetKeyDown(pauseKey))
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
                if (Input.GetKeyDown(pauseKey))
                {
                    //selectorSprite.transform.position = startingSelectorPosition;
                    selectorSprite.gameObject.SetActive(true);
                    selectorSprite.transform.localPosition = pauseMenuStartPosition;
                    selectorSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                    pauseIndex = 0;
                    uiIndex = -2;
                }
            }
            // Main Menu
            else if (uiIndex == 0)
            {
                if (!controlsVisible)
                {
                    if (Input.GetKeyDown(replayTitleKey))
                    {
                        replayTitle();
                    }
                    mainMenuText.SetActive(true);
                    selectorSprite.SetActive(true);
                    //authorText.SetActive(true);
                    if (Input.GetKeyDown(rotateRightKey))
                    {
                        rotateSelector(true);
                    }
                    if (Input.GetKeyDown(rotateLeftKey))
                    {
                        rotateSelector(false);
                    }
                    //if (Input.GetKeyDown(KeyCode.UpArrow) && mainMenuIndex > 0)
                    //{
                    //    //selectorSprite.transform.Translate(new Vector3(0, displace));
                    //    selectorSprite.transform.position += new Vector3(0, mainMenuDisplace);
                    //    --mainMenuIndex;
                    //    audioSource.PlayOneShot(slideRotateClip);

                    //}
                    //if (Input.GetKeyDown(KeyCode.DownArrow) && mainMenuIndex < mainMenuIndexMax)
                    //{
                    //    //selectorSprite.transform.Translate(new Vector3(0, -displace));
                    //    selectorSprite.transform.position -= new Vector3(0, mainMenuDisplace);
                    //    ++mainMenuIndex;
                    //    audioSource.PlayOneShot(slideRotateClip);
                    //}
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, displace));
                        selectorSprite.transform.position += new Vector3(0, mainMenuDisplace);
                        --mainMenuIndex;
                        audioSource.PlayOneShot(slideRotateClip);

                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        //selectorSprite.transform.Translate(new Vector3(0, -displace));
                        selectorSprite.transform.position -= new Vector3(0, mainMenuDisplace);
                        ++mainMenuIndex;
                        audioSource.PlayOneShot(slideRotateClip);
                    }
                    if(mainMenuIndex < 0)
                    {
                        mainMenuIndex = mainMenuIndexMax;
                    }
                    else if(mainMenuIndex > mainMenuIndexMax)
                    {
                        mainMenuIndex = 0;
                    }

                    selectorSprite.transform.localPosition = mainMenuStartPosition - new Vector3(0, mainMenuIndex * mainMenuDisplace);

                }
                if (Input.GetKeyDown(KeyCode.Space) && !displayingCredits)
                {
                    //if(mainMenuIndex == 0)
                    //{
                    //    mainMenuText.SetActive(false);
                    //    selectorSprite.SetActive(false);
                    //    gameOverText.SetActive(false);
                    //    newHighScoreText.SetActive(false);
                    //    authorText.SetActive(false);
                    //    scoreText.GetComponent<Text>().text = "Rows Cleared: 0";
                    //    gameBoard.GetComponent<Board>().startGame();
                    //    scoreText.SetActive(true);
                    //    uiIndex = -1;
                    //}
                    audioSource.PlayOneShot(newMenuClip, 0.1f);
                    if (mainMenuIndex == 0)
                    {
                        uiIndex = 1;
                        //playMenuIndex = 0;
                        //selectorSprite.transform.localPosition = playMenuStartPosition;
                        //selectorSprite.transform.localPosition = playMenuStartPosition;
                        selectorSprite.transform.localPosition = playMenuStartPosition - new Vector3(0, playMenuIndex * playMenuDisplace);
                        gameTimer.gameObject.SetActive(false);
                        marathonLevelText.gameObject.SetActive(false);
                        scoreText.SetActive(false);
                        newHighScoreText.SetActive(false);
                        //highScoreText.SetActive(false);
                        gameOverText.SetActive(false);
                        highscoreRows.SetActive(false);
                        highscoreTime.SetActive(false);
                        if(playMenuIndex == 0)
                        {
                            selectorSprite.transform.localPosition = playMenuStartPositionBase;
                        }
                    }
                    else if (mainMenuIndex == 1)
                    {
                        uiIndex += 4;
                    }
                    else if (mainMenuIndex == 2)
                    {
                        creditsBoard.SetActive(true);
                        displayingCredits = true;
                        
                    }
                    else if (mainMenuIndex == 3)
                    {
                        //Debug.Log("Quitting");
                        Application.Quit();
                    }
                    //else if (mainMenuIndex == 1)
                    //{
                    //    controlsVisible = true;
                    //    controlsInfoBox.SetActive(true);
                    //}
                    //else if (mainMenuIndex == 2)
                    //{
                    //    uiIndex += 3;
                    //}
                    //else if (mainMenuIndex == 3)
                    //{
                    //    //Debug.Log("Quitting");
                    //    Application.Quit();
                    //}
                }
                //if (Input.GetKeyDown(KeyCode.Escape))
                //{
                //    controlsVisible = false;
                //    controlsInfoBox.SetActive(false);
                //}
                if (Input.GetKeyDown(KeyCode.Escape) && displayingCredits)
                {
                    creditsBoard.SetActive(false);
                    displayingCredits = false;
                }
            }
            // Game Mode Menu
            else if (uiIndex == 1)
            {
                mainMenuText.SetActive(false);
                
                playMenuText.SetActive(true);

                if (Input.GetKeyDown(replayTitleKey))
                {
                    replayTitle();
                    playMenuText.SetActive(false);
                    selectorSprite.transform.localPosition = mainMenuStartPosition;
                    uiIndex = 0;
                }
                if (Input.GetKeyDown(rotateRightKey))
                {
                    rotateSelector(true);
                }
                if (Input.GetKeyDown(rotateLeftKey))
                {
                    rotateSelector(false);
                }
                //if (Input.GetKeyDown(KeyCode.UpArrow) && playMenuIndex > 0)
                //{
                //    //selectorSprite.transform.Translate(new Vector3(0, displace));
                //    selectorSprite.transform.position += new Vector3(0, playMenuDisplace);
                //    audioSource.PlayOneShot(slideRotateClip);
                //    --playMenuIndex;

                //}
                //if (Input.GetKeyDown(KeyCode.DownArrow) && playMenuIndex < playMenuIndexMax)
                //{
                //    //selectorSprite.transform.Translate(new Vector3(0, -displace));
                //    selectorSprite.transform.position -= new Vector3(0, playMenuDisplace);
                //    audioSource.PlayOneShot(slideRotateClip);
                //    ++playMenuIndex;
                //}
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //selectorSprite.transform.Translate(new Vector3(0, displace));
                    //selectorSprite.transform.position += new Vector3(0, playMenuDisplace);
                    audioSource.PlayOneShot(slideRotateClip);
                    --playMenuIndex;

                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //selectorSprite.transform.Translate(new Vector3(0, -displace));
                    //selectorSprite.transform.position -= new Vector3(0, playMenuDisplace);
                    audioSource.PlayOneShot(slideRotateClip);
                    ++playMenuIndex;
                }
                if(playMenuIndex < 0)
                {
                    playMenuIndex = playMenuIndexMax;
                }
                else if (playMenuIndex > playMenuIndexMax)
                {
                    playMenuIndex = 0;
                }
                selectorSprite.transform.localPosition = playMenuStartPosition - new Vector3(0, playMenuIndex * playMenuDisplace);
                gameModeDescriptionText.text = gameModeDescriptions[playMenuIndex];
                if (Input.GetKeyDown(KeyCode.Space))
                {

                    if (playMenuIndex == 0)
                    {
                        playMenuText.SetActive(false);
                        mainMenuText.SetActive(false);
                        selectorSprite.SetActive(false);
                        gameOverText.SetActive(false);
                        newHighScoreText.SetActive(false);
                        authorText.SetActive(false);
                        settings.setGameMode(0);
                        settings.updateAll();
                        scoreText.GetComponent<Text>().text = "Rows Cleared: 0";
                        gameBoard.GetComponent<Board>().startGame();
                        marathonLevelText.gameObject.SetActive(false);
                        scoreText.SetActive(true);
                        //playMenuStartPosition = selectorSprite.transform.localPosition;
                        uiIndex = -1;
                        Debug.Log("Rest Selected");
                    }
                    else if (playMenuIndex == 1)
                    {
                        playMenuText.SetActive(false);
                        mainMenuText.SetActive(false);
                        selectorSprite.SetActive(false);
                        gameOverText.SetActive(false);
                        newHighScoreText.SetActive(false);
                        authorText.SetActive(false);
                        settings.setGameMode(1);
                        settings.updateAll();
                        scoreText.GetComponent<Text>().text = "Rows Cleared: 0";
                        gameBoard.GetComponent<Board>().startGame();
                        scoreText.SetActive(true);
                        gameTimer.gameObject.SetActive(true);
                        marathonLevelText.gameObject.SetActive(false);
                        //playMenuStartPosition = selectorSprite.transform.localPosition;
                        uiIndex = -1;
                        Debug.Log("Marathon Selected");
                    }
                    else if (playMenuIndex == 2)
                    {
                        playMenuText.SetActive(false);
                        mainMenuText.SetActive(false);
                        selectorSprite.SetActive(false);
                        gameOverText.SetActive(false);
                        newHighScoreText.SetActive(false);
                        authorText.SetActive(false);
                        settings.setGameMode(2);
                        settings.updateAll();
                        scoreText.GetComponent<Text>().text = "Rows Cleared: 0";
                        gameBoard.GetComponent<Board>().startGame();
                        scoreText.SetActive(true);
                        //playMenuStartPosition = selectorSprite.transform.localPosition;
                        gameTimer.gameObject.SetActive(true);
                        marathonLevelText.gameObject.SetActive(true);
                        updateMarathonLevelText(0);
                        uiIndex = -1;
                        Debug.Log("Sprint Selected");
                    }
                    else if (playMenuIndex == 3)
                    {
                        uiIndex = 0;
                        playMenuText.SetActive(false);
                        selectorSprite.transform.localPosition = mainMenuStartPosition;
                        playMenuIndex = 0;
                        audioSource.PlayOneShot(newMenuClip, 0.1f);
                        //selectorSprite.transform.localPosition = playMenuStartPositionBase;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    uiIndex = 0;
                    playMenuText.SetActive(false);
                    selectorSprite.transform.localPosition = mainMenuStartPosition;
                }
            }
            // Main Menu Settings
            else if (uiIndex >= 2)
            {
                settingsBoard.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Escape) && !settingsBoard.GetComponent<SettingsMenu>().getListeningFlag())
                {
                    settingsBoard.GetComponent<SettingsMenu>().updateAllSettings();
                    //settingsBoard.GetComponent<SettingsMenu>().updateSettingsDasArr();
                    //settingsBoard.GetComponent<SettingsMenu>().updateControls();
                    //settingsBoard.GetComponent<SettingsMenu>().resestSelectorPosition();
                    //settingsBoard.GetComponent<SettingsMenu>().setLockDelayFlag();
                    setKeys(settingsBoard.GetComponent<SettingsMenu>().getCurrentControls());
                    gameBoard.GetComponent<Board>().updatePieceSettings();
                    uiIndex -= 4;
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
