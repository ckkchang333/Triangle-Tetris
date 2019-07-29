using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour {



    public GameObject gameBoard;

    public GameObject UI;

    public List<KeyCode> defaultControls;

    public List<KeyCode> currentControls;

    private int rotateLeftControlIndex = 0;
    private int rotateRightControlIndex = 1;
    private int shiftLeftControlIndex = 2;
    private int shiftRightControlIndex = 3;
    private int softDropControlIndex = 4;
    private int lockInControlIndex = 5;
    private int hardDropControlIndex = 6;
    private int holdControlIndex = 7;
    private int ghostControlIndex = 8;
    private int pauseControlIndex = 9;
    private int quickRestartControlIndex = 10;


    public int leftDasValue = 10;
    public int leftArrValue = 6;
    public int rightDasValue = 10;
    public int rightArrValue = 6;

    public bool lockDelayFlag = true;

    public int gameMode = -1;

    public void updateAll()
    {
        updateBoardDasArr();
        updateBoardGameMode();
        updateBoardLockDelay();
        updateAllControls();
        updateSettingsBoardDasArr();
        updateSettingsControls();
    }

    public void setGameMode(int newGameMode)
    {
        gameMode = newGameMode;
    }

    public void updateBoardGameMode()
    {
        gameBoard.GetComponent<Board>().setGameMode(gameMode);
    }

    public void setDasArr(int newLeftDas, int newLeftArr, int newRightDas, int newRightArr)
    {
        leftDasValue = newLeftDas;
        leftArrValue = newLeftArr;
        rightDasValue = newRightDas;
        rightArrValue = newRightArr;
    }

    public void updateSettingsBoardDasArr()
    {
        UI.transform.Find("Settings Board").GetComponent<SettingsMenu>().setDasArr(leftDasValue, leftArrValue, rightDasValue, rightArrValue);
    }


    public void updateSettingsControls()
    {
        UI.transform.Find("Settings Board").GetComponent<SettingsMenu>().setControls(defaultControls);
    }

    public void updateBoardDasArr()
    {
        gameBoard.GetComponent<Board>().setDasArr(leftDasValue, leftArrValue, rightDasValue, rightArrValue);
    }


    public void setLockDelay(bool toggle)
    {
        lockDelayFlag = toggle;
    }

    public void updateBoardLockDelay()
    {
        gameBoard.GetComponent<Board>().setLockDelay(lockDelayFlag);
    }

    public void setControls(List<KeyCode> newControls)
    {
        currentControls = new List<KeyCode>(newControls);
    }

    public void updateAllControls()
    {
        gameBoard.GetComponent<Board>().setControlsKeys(currentControls);
        UI.GetComponent<UI>().setKeys(currentControls);
    }

    // Use this for initialization
    void Start ()
    {
        defaultControls.Add(KeyCode.Z);
        defaultControls.Add(KeyCode.X);
        defaultControls.Add(KeyCode.LeftArrow);
        defaultControls.Add(KeyCode.RightArrow);
        defaultControls.Add(KeyCode.DownArrow);
        defaultControls.Add(KeyCode.Space);
        defaultControls.Add(KeyCode.DownArrow);
        defaultControls.Add(KeyCode.LeftShift);
        defaultControls.Add(KeyCode.G);
        defaultControls.Add(KeyCode.Escape);
        defaultControls.Add(KeyCode.F10);

        currentControls = new List<KeyCode>(defaultControls);
        
        updateAll();


        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.targetFrameRate != 60)
        {
            Application.targetFrameRate = 60;
        }
    }
}
