using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject mainMenuSelectorSprite;
    public int mainMenuIndex = 0;
    public int mainMenuxMaxIndex = 2;
    public float displace;

    [Header("Controls")]
    public GameObject controlsInfoBox;

    [Header("Other")]
    public GameObject scoreText;

    public bool active;
    public int uiIndex;


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
            mainMenuSelectorSprite.transform.Rotate(new Vector3(0, 0, -90));
        }
        else
        {
            mainMenuSelectorSprite.transform.Rotate(new Vector3(0, 0, 90));
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(active)
        {
            //Debug.Log(uiIndex);
            mainMenu.SetActive(uiIndex == 0);
            mainMenuSelectorSprite.SetActive(uiIndex == 0);
            if (uiIndex == 0)
            {
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
                    //mainMenuSelectorSprite.transform.Translate(new Vector3(0, displace));
                    mainMenuSelectorSprite.transform.position += new Vector3(0, displace);
                    mainMenuIndex = --mainMenuIndex;

                }
                if (Input.GetKeyDown(KeyCode.S) && mainMenuIndex < mainMenuxMaxIndex)
                {
                    //mainMenuSelectorSprite.transform.Translate(new Vector3(0, -displace));
                    mainMenuSelectorSprite.transform.position -= new Vector3(0, displace);
                    mainMenuIndex = ++mainMenuIndex;
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if(mainMenuIndex == 0)
                    {
                        mainMenu.SetActive(false);
                        mainMenuSelectorSprite.SetActive(false);
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
            if(uiIndex == 1)
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
