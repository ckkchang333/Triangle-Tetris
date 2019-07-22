
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public int leftDasValue = 10;
    public int leftArrValue = 6;
    public int rightDasValue = 10;
    public int rightArrValue = 6;
    public int dasMin = 1;
    public int dasMax = 30;
    public int arrMin = 1;
    public int arrMax = 30;

    public Text leftDasText;
    public Text leftArrText;
    public Text rightDasText;
    public Text rightArrText;

    public GameObject selectorKeys;
    public GameObject selectorTriangles;
    public GameObject selectorReset;

    public GameObject lockDelaySelector;
    public Text lockDelayText;
    public bool lockDelayFlag = true;

    public float displaceKeys;
    public float displaceParameters;

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


    private Vector3 startingSelectorKeysPosition;
    private Vector3 startingTriangleSelectorPosition;


    public int menuIndex;

    public GameObject gameBoard;

    public GameObject spaceSelectPrompt;
    public GameObject spaceRebindPrompt;

    public GameObject controlsValues;

    private bool inputListening = false;


    public KeyCode key;

    public Text keyText;


    public void setLockDelayFlag()
    {
        gameBoard.GetComponent<Board>().setLockDelayFlag(lockDelayFlag);
    }

    void updateControlKeysText()
    {
        for (int i = 0; i < 11; ++i)
        {
            controlsValues.transform.GetChild(i).GetComponent<Text>().text = (currentControls[i]).ToString();
        }
    }

    public List<KeyCode> getCurrentControls()
    {
        return currentControls;
    }

    void detectKey()
    {
        if(Input.anyKeyDown)
        {
            foreach (KeyCode current in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(current))
                {
                    //Debug.Log("KeyCode down: " + current);
                    //keyText.text = current.ToString();
                    //key = current;
                    controlsValues.transform.GetChild(menuIndex).GetComponent<Text>().text = current.ToString();
                    currentControls[menuIndex] = current;
                    
                }
            }
            inputListening = false;
        }
    }

    public bool getListeningFlag()
    {
        return inputListening;
    }

    void changeKeysSelectorColor(Color newColor)
    {
        selectorKeys.transform.GetChild(0).GetComponent<SpriteRenderer>().color = newColor;
    }

    void updateText(Text toChange, string newText)
    {
        toChange.text = newText;
    }

    public void updateGameBoardDasAndArr()
    {
        gameBoard.GetComponent<Board>().setDasAndArr(leftDasValue, leftArrValue, rightDasValue, rightArrValue);
    }

    public void updateControls()
    {
        gameBoard.GetComponent<Board>().setControlsKeys(currentControls);
        
    }

    public void resestSelectorPosition()
    {
        menuIndex = 0;
        selectorTriangles.transform.position = startingTriangleSelectorPosition;
        selectorKeys.transform.position = startingSelectorKeysPosition;
        selectorTriangles.SetActive(true);
        selectorReset.SetActive(false);
    }

    // Use this for initialization
    void Start ()
    {
        startingSelectorKeysPosition = selectorKeys.transform.position;
        startingTriangleSelectorPosition = selectorTriangles.transform.position;


        leftDasText.text = leftDasValue.ToString();
        leftArrText.text = leftArrValue.ToString();
        rightDasText.text = rightDasValue.ToString();
        rightArrText.text = rightArrValue.ToString();

        //defaultControls = new List<KeyCode>(11);

        //defaultControls[rotateLeftControlIndex] = KeyCode.Z;
        //defaultControls[rotateRightControlIndex] = KeyCode.X;
        //defaultControls[shiftLeftControlIndex] = KeyCode.LeftArrow;
        //defaultControls[shiftRightControlIndex] = KeyCode.RightArrow;
        //defaultControls[softDropControlIndex] = KeyCode.DownArrow;
        //defaultControls[hardDropControlIndex] = KeyCode.Space;
        //defaultControls[lockInControlIndex] = KeyCode.DownArrow;
        //defaultControls[holdControlIndex] = KeyCode.LeftShift;
        //defaultControls[ghostControlIndex] = KeyCode.G;
        //defaultControls[pauseControlIndex] = KeyCode.P;
        //defaultControls[quickRestartControlIndex] = KeyCode.F10;
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
        updateControlKeysText();


        updateGameBoardDasAndArr();
        updateControls();

        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(inputListening)
        {
            changeKeysSelectorColor(Color.red);
            detectKey();
        }
        else
        {
            changeKeysSelectorColor(Color.green);
            if (menuIndex <= 10)
            {
                selectorKeys.SetActive(true);
                selectorTriangles.SetActive(false);
                selectorReset.SetActive(false);
                spaceSelectPrompt.SetActive(false);
                lockDelaySelector.SetActive(false);
                spaceRebindPrompt.SetActive(true);
            }
            else if (menuIndex == 11)
            {
                selectorKeys.SetActive(false);
                selectorTriangles.SetActive(false);
                selectorReset.SetActive(false);
                spaceSelectPrompt.SetActive(true);
                lockDelaySelector.SetActive(true);
                spaceRebindPrompt.SetActive(false);
            }
            else if (menuIndex >= 11 && menuIndex <= 15)
            {
                selectorKeys.SetActive(false);
                selectorTriangles.SetActive(true);
                selectorReset.SetActive(false);
                spaceSelectPrompt.SetActive(false);
                lockDelaySelector.SetActive(false);
                spaceRebindPrompt.SetActive(false);
            }
            else if (menuIndex == 16)
            {
                selectorKeys.SetActive(false);
                selectorTriangles.SetActive(false);
                selectorReset.SetActive(true);
                spaceSelectPrompt.SetActive(true);
                lockDelaySelector.SetActive(false);
                spaceRebindPrompt.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && menuIndex < 16)
            {
                ++menuIndex;
                if (menuIndex <= 10)
                {
                    selectorKeys.transform.position -= new Vector3(0, displaceKeys);
                }
                else if (menuIndex > 12 && menuIndex <= 15)
                {
                    selectorTriangles.transform.position -= new Vector3(0, displaceParameters);
                }

            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && menuIndex > 0)
            {
                --menuIndex;
                if (menuIndex < 10)
                {
                    selectorKeys.transform.position += new Vector3(0, displaceKeys);
                }
                else if (menuIndex >= 12 && menuIndex < 15)
                {
                    selectorTriangles.transform.position += new Vector3(0, displaceParameters);
                }
                //if (menuIndex == 4)
                //{
                //    selectorTriangles.SetActive(true);
                //    selectorReset.SetActive(false);
                //    spaceSelectPrompt.SetActive(false);
                //}
                //else
                //{
                //    selectorTriangles.transform.position += new Vector3(0, displaceParameters);
                //}

            }


            if (menuIndex == 0)
            {
                if (leftDasValue == dasMin)
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (leftDasValue == dasMax)
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else if (menuIndex == 1)
            {
                if (leftArrValue == arrMin)
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (leftArrValue == arrMax)
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else if (menuIndex == 2)
            {
                if (rightDasValue == dasMin)
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (rightDasValue == dasMax)
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else if (menuIndex == 3)
            {
                if (rightArrValue == arrMin)
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (rightArrValue == arrMax)
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    selectorTriangles.transform.GetChild(1).gameObject.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (menuIndex == 12)
                {
                    if (leftDasValue > dasMin)
                    {
                        --leftDasValue;
                        leftDasText.text = leftDasValue.ToString();
                    }
                }
                else if (menuIndex == 13)
                {
                    if (leftArrValue > arrMin)
                    {
                        --leftArrValue;
                        leftArrText.text = leftArrValue.ToString();
                    }
                }
                else if (menuIndex == 14)
                {
                    if (rightDasValue > dasMin)
                    {
                        --rightDasValue;
                        rightDasText.text = rightDasValue.ToString();
                    }
                }
                else if (menuIndex == 15)
                {
                    if (rightArrValue > arrMin)
                    {
                        --rightArrValue;
                        rightArrText.text = rightArrValue.ToString();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (menuIndex == 12)
                {
                    if (leftDasValue < dasMax)
                    {
                        ++leftDasValue;
                        leftDasText.text = leftDasValue.ToString();
                    }
                }
                else if (menuIndex == 13)
                {
                    if (leftArrValue < arrMax)
                    {
                        ++leftArrValue;
                        leftArrText.text = leftArrValue.ToString();
                    }
                }
                else if (menuIndex == 14)
                {
                    if (rightDasValue < dasMax)
                    {
                        ++rightDasValue;
                        rightDasText.text = rightDasValue.ToString();
                    }
                }
                else if (menuIndex == 15)
                {
                    if (rightArrValue < arrMax)
                    {
                        ++rightArrValue;
                        rightArrText.text = rightArrValue.ToString();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (menuIndex >= 0 && menuIndex <= 10)
                {
                    inputListening = true;
                }
                else if(menuIndex == 11)
                {
                    lockDelayFlag = !lockDelayFlag;
                    if(lockDelayFlag)
                    {
                        lockDelayText.text = "Enabled";
                    }
                    else
                    {
                        lockDelayText.text = "Disabled";
                    }
                }
                else if (menuIndex == 16)
                {
                    leftDasValue = 10;
                    leftDasText.text = leftDasValue.ToString();
                    leftArrValue = 5;
                    leftArrText.text = leftArrValue.ToString();
                    rightDasValue = 10;
                    rightDasText.text = rightDasValue.ToString();
                    rightArrValue = 5;
                    rightArrText.text = rightArrValue.ToString();

                    lockDelayFlag = true;

                    lockDelayText.text = "Enabled";

                    currentControls = new List<KeyCode>(defaultControls);
                    updateControlKeysText();
                }
            }
        }
        //if(Input.anyKeyDown)
        //{
        //    detectKey();
        //}
        
    }
}
