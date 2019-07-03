using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    [Header("Main Menu")]
    public GameObject mainMenuText;
    public int mainMenuIndex = 0;
    public int mainMenuMaxIndex = 2;
    public float displace;

    [Header("Controls")]
    public GameObject controlsInfoBox;

    [Header("Pause Menu")]
    public GameObject pauseText;
    public int pauseMaxIndex = 1;
    private int pauseIndex = 0;

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

    private void rotateSelector(bool right)
    {
        if(right)
        {
            selectorSprite.transform.Rotate(new Vector3(0, 0, -90));
        }
        else
        {
            selectorSprite.transform.Rotate(new Vector3(0, 0, 90));
        }
    }

	// Use this for initialization
	void Start () {
        startingSelectorPosition = selectorSprite.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(active)
        {
            //Debug.Log(uiIndex);

            // Game Paused
            if (uiIndex == -2)
            {
                pauseText.SetActive(true);
                selectorSprite.SetActive(true);
                if (Input.GetKeyDown(KeyCode.P))
                {
                    pauseText.SetActive(true);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    rotateSelector(true);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    rotateSelector(false);
                }
                if (Input.GetKeyDown(KeyCode.W) && pauseIndex > 0)
                {
                    //selectorSprite.transform.Translate(new Vector3(0, displace));
                    selectorSprite.transform.position += new Vector3(0, displace);
                    pauseIndex = --pauseIndex;

                }
                if (Input.GetKeyDown(KeyCode.S) && pauseIndex < pauseMaxIndex)
                {
                    //selectorSprite.transform.Translate(new Vector3(0, -displace));
                    selectorSprite.transform.position -= new Vector3(0, displace);
                    pauseIndex = ++pauseIndex;
                }
                if(Input.GetKey(KeyCode.Space))
                {
                    if(pauseIndex == 0)
                    {
                        pauseText.SetActive(false);
                        selectorSprite.SetActive(false);
                        gameBoard.GetComponent<Board>().resetGame();
                        gameBoard.GetComponent<Board>().startGame();
                        uiIndex = -1;
                    }
                }
                if(Input.GetKeyDown(KeyCode.P))
                {
                    pauseText.SetActive(false);
                    selectorSprite.SetActive(false);
                }
            }
            // Playing the Game
            if (uiIndex == -1)
            {
                if(Input.GetKeyDown(KeyCode.P))
                {
                    selectorSprite.transform.position = startingSelectorPosition;
                    selectorSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                    pauseIndex = 0;
                    uiIndex = -2;
                }
            }
            else if (uiIndex == 0)
            {
                mainMenuText.SetActive(true);
                selectorSprite.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    rotateSelector(true);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    rotateSelector(false);
                }
                if (Input.GetKeyDown(KeyCode.W) && mainMenuIndex > 0)
                {
                    //selectorSprite.transform.Translate(new Vector3(0, displace));
                    selectorSprite.transform.position += new Vector3(0, displace);
                    mainMenuIndex = --mainMenuIndex;

                }
                if (Input.GetKeyDown(KeyCode.S) && mainMenuIndex < mainMenuMaxIndex)
                {
                    //selectorSprite.transform.Translate(new Vector3(0, -displace));
                    selectorSprite.transform.position -= new Vector3(0, displace);
                    mainMenuIndex = ++mainMenuIndex;
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if(mainMenuIndex == 0)
                    {
                        mainMenuText.SetActive(false);
                        selectorSprite.SetActive(false);
                        gameBoard.GetComponent<Board>().startGame();
                        scoreText.SetActive(true);
                        uiIndex = -1;
                    }
                    else if (mainMenuIndex == 1)
                    {
                        uiIndex = 1;
                        //Debug.Log("Displaying Controls Box");
                        //Debug.Log(uiIndex);
                        //Debug.Log(uiIndex == 1);
                        controlsInfoBox.SetActive(true);
                        //Debug.Log("Setting uiIndex");
                        return;
                        //Debug.Log("uiIndex Set");
                        //Debug.Log(uiIndex);
                    }
                    else if (mainMenuIndex == 2)
                    {
                        //Debug.Log("Quitting");
                        Application.Quit();
                    }
                }
            }
            else if(uiIndex == 1)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    uiIndex = 0;
                    controlsInfoBox.SetActive(false);
                }
            }
            //Debug.Log(uiIndex);
        }
    }
}
