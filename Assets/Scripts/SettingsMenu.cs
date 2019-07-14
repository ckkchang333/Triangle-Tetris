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

    public GameObject selectorTriangles;
    public GameObject selectorReset;

    public float displace;

    private Vector3 startingTriangleSelectorPosition;


    private int menuIndex;

    public GameObject gameBoard;

    public GameObject spaceSelectPrompt;

    void updateText(Text toChange, int value)
    {
        toChange.text = value.ToString();
    }

    public void updateGameBoardDasAndArr()
    {
        gameBoard.GetComponent<Board>().setDasAndArr(leftDasValue, leftArrValue, rightDasValue, rightArrValue);
    }

    public void resestSelectorPosition()
    {
        menuIndex = 0;
        selectorTriangles.transform.position = startingTriangleSelectorPosition;
        selectorTriangles.SetActive(true);
        selectorReset.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        leftDasText.text = leftDasValue.ToString();
        leftArrText.text = leftArrValue.ToString();
        rightDasText.text = rightDasValue.ToString();
        rightArrText.text = rightArrValue.ToString();

        startingTriangleSelectorPosition = selectorTriangles.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && menuIndex < 4)
        {
            ++menuIndex;
            if (menuIndex < 4)
            {
                selectorTriangles.transform.position -= new Vector3(0, displace);
            }
            else
            {
                selectorTriangles.SetActive(false);
                selectorReset.SetActive(true);
                spaceSelectPrompt.SetActive(true);
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && menuIndex > 0)
        {
            if (menuIndex == 4)
            {
                selectorTriangles.SetActive(true);
                selectorReset.SetActive(false);
                spaceSelectPrompt.SetActive(false);
            }
            else
            {
                selectorTriangles.transform.position += new Vector3(0, displace);
            }
            --menuIndex;

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
            if (menuIndex == 0)
            {
                if (leftDasValue > dasMin)
                {
                    updateText(leftDasText, --leftDasValue);
                }
            }
            else if (menuIndex == 1)
            {
                if (leftArrValue > arrMin)
                {
                    updateText(leftArrText, --leftArrValue);
                }
            }
            else if (menuIndex == 2)
            {
                if (rightDasValue > dasMin)
                {
                    updateText(rightDasText, --rightDasValue);
                }
            }
            else if (menuIndex == 3)
            {
                if (rightArrValue > arrMin)
                {
                    updateText(rightArrText, --rightArrValue);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (menuIndex == 0)
            {
                if (leftDasValue < dasMax)
                {
                    updateText(leftDasText, ++leftDasValue);
                }
            }
            else if (menuIndex == 1)
            {
                if (leftArrValue < arrMax)
                {
                    updateText(leftArrText, ++leftArrValue);
                }
            }
            else if (menuIndex == 2)
            {
                if (rightDasValue < dasMax)
                {
                    updateText(rightDasText, ++rightDasValue);
                }
            }
            else if (menuIndex == 3)
            {
                if (rightArrValue < arrMax)
                {
                    updateText(rightArrText, ++rightArrValue);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (menuIndex == 4)
            {
                updateText(leftDasText, leftDasValue = 10);
                updateText(leftArrText, leftArrValue = 5);
                updateText(rightDasText, rightDasValue = 10);
                updateText(rightArrText, rightArrValue = 5);
            }
        }
    }
}
