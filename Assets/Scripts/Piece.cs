using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ListWrapperInt
{

    public List<int> myList;
}

public class Piece : MonoBehaviour {

    public Board gameBoard;

    [Header("Piece Parameters")]
    public int coreTriangle;
    public int orientationState;
    public List<int> trianglesIndices;
    public List<ListWrapperInt> orientationsDeltas;
    public float dropTimeIntervalBase = 60;
    public float dropTimeInterval;
    private float timer;
    public int pieceID;
    public bool active;
    public bool letter;
    public bool obilteration;
    public bool printRotateDebug;
    public int leftDelayedAutoshift;
    public int rightDelayedAutoshift;
    public int leftAutoRepeatRate;
    public int rightAutoRepeatRate;
    public int frameCounter = -1;
    private bool firstDelayflag = true;
    public List<KeyCode> controlsKeys;
    private int rotateLeftIndex = 0;
    private int rotateRightIndex = 1;
    private int shiftLeftIndex = 2;
    private int shiftRightIndex = 3;
    private int softDropIndex = 4;
    private int hardDropIndex = 5;
    private int lockInIndex = 6;

    private bool lockDelayFlag = true;

    public bool suspend = false;
    
    public GameObject pieceSprite;

    public Color pieceColor = Color.red;

    public AudioSource audioSource;
    public AudioClip slideRotateClip;
    public float slideRotateClipVolume;
    public AudioClip hardDropClip;
    public float hardDropClipVolume;
    public AudioClip spawnClip;
    public float spawnClipVolume;

    public bool spinDirectionCheckFlag = false;
    public int clockwiseLeft = -1;
    public int counterwiseLeft = -1;
    private bool ready = true;
    //public bool isTrap = false;

    public void setSuspend(bool toggle)
    {
        suspend = toggle;
    }

    public void setLockDelay(bool toggle)
    {
        lockDelayFlag = toggle;
    }

    public void setControlsKeys(List<KeyCode> newKeys)
    {
        controlsKeys = new List<KeyCode>(newKeys.GetRange(0, lockInIndex + 1));
    }


    public int getPieceID()
    {
        return pieceID;
    }

    public List<int> getTriangleIndices()
    {
        return new List<int>(trianglesIndices);
    }

    public int getCoreTriangle()
    {
        return coreTriangle;
    }

    int getColumnIndex(int triangleIndex)
    {
        return (triangleIndex % (gameBoard.width * 4)) / 4;
    }


    int getRowIndex(int triangleIndex)
    {
        return triangleIndex / (gameBoard.width * 4);
    }

    public GameObject getSprite()
    {
        return pieceSprite;
    }

    void fallOnce()
    {
        //if (Input.GetKey(controlsKeys[softDropIndex]) && !letter)
        if (Input.GetKey(controlsKeys[softDropIndex]) && !letter)
        {
            audioSource.PlayOneShot(slideRotateClip, slideRotateClipVolume);
        }
        List<int> fallTriangleIndices = getNewTriangleIndices(0, -1);
        for (int i = 0; i < trianglesIndices.Count; ++i)
        {
            int current = trianglesIndices[i];
            if (current % 4 == 0)
            {
                int oppositeBelowTriangleIndex = current - (gameBoard.getWidth() * 4 - 2);
                if (!fallTriangleIndices.Contains(oppositeBelowTriangleIndex) && oppositeBelowTriangleIndex >= 0)
                {
                    //Debug.Log(oppositeBelowTriangleIndex);
                    fallTriangleIndices.Insert(i, oppositeBelowTriangleIndex);
                    //++i;
                }
            }
            else if (current % 4 == 2)
            {
                int oppositeBelowTriangleIndex = current - 2;
                if (!fallTriangleIndices.Contains(oppositeBelowTriangleIndex) && oppositeBelowTriangleIndex >= 0)
                {
                    //Debug.Log(oppositeBelowTriangleIndex);
                    fallTriangleIndices.Insert(i, oppositeBelowTriangleIndex);
                    //++i;
                }
            }
        }
        if (gameBoard.checkEmpty(fallTriangleIndices))
        {
            gameBoard.emptyTriangles(trianglesIndices);
            coreTriangle -= gameBoard.width * 4;
            updateTriangleIndices();
        }
        else
        {
            if(!suspend || (suspend && Input.GetKeyDown(controlsKeys[lockInIndex])))
            {
                gameBoard.setPerm(trianglesIndices);
                Destroy(this.gameObject);
            }
        }
    }

    public void resetPosition()
    {
        if (!letter)
        {
            orientationState = 0;
            coreTriangle = (gameBoard.getHeight() - 2) * (gameBoard.getWidth() * 4) + (gameBoard.getWidth() / 2 - (trianglesIndices.Count > 8 ? 2 : 1)) * 4;
            updateTriangleIndices();
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                if (trianglesIndices[i] >= (gameBoard.getHeight() - 1) * 4 * gameBoard.getWidth())
                {
                    break;
                }
                if (i == trianglesIndices.Count - 1)
                {
                    coreTriangle += gameBoard.getWidth() * 4;
                }
                updateTriangleIndices();
            }
        }
        else
        {
            coreTriangle = (gameBoard.getHeight() - 4) * (gameBoard.getWidth() * 4) + (gameBoard.getWidth() / 2) * 4;
        }
        if (!gameBoard.checkEmpty(trianglesIndices))
        {
            gameBoard.setGameOver(true);
        }
    }

    List<int> getNewTriangleIndices(int newX, int newY, int newOrientationDelta = 0)
    {
        List<int> newTriangleIndices = new List<int>();
        int newCoreTriangle = coreTriangle - gameBoard.width * 4 * -newY;
        newCoreTriangle += 4 * newX;
        int updatedOrientationState = orientationState + newOrientationDelta;
        if(updatedOrientationState < 0)
        {
            updatedOrientationState += orientationsDeltas.Count;
        }
        else if (updatedOrientationState > orientationsDeltas.Count - 1)
        {
            updatedOrientationState -= orientationsDeltas.Count;
        }
        List<int> deltas = orientationsDeltas[updatedOrientationState].myList;
        for (int i = 0; i < deltas.Count; ++i)
        {
            newTriangleIndices.Add(newCoreTriangle + deltas[i]);
        }
        return newTriangleIndices;
    }

    void updateTriangleIndices()
    {
        if (orientationState < 0)
        {
            orientationState += orientationsDeltas.Count;
        }
        else if(orientationState > orientationsDeltas.Count - 1)
        {
            orientationState -= orientationsDeltas.Count;
        }
        List<int> currentDeltas = orientationsDeltas[orientationState].myList;
        trianglesIndices.Clear();
        for (int i = 0; i < currentDeltas.Count; ++i)
        {
            trianglesIndices.Add(coreTriangle + currentDeltas[i]);
        }
    }


    public void setDasAndArr(int leftDASvalue, int leftARRvalue, int rightDASvalue, int rightARRvalue)
    {
        leftDelayedAutoshift = leftDASvalue;
        leftAutoRepeatRate = leftARRvalue;
        rightDelayedAutoshift = rightDASvalue;
        rightAutoRepeatRate = rightARRvalue;
    }


    void rotate()
    {
        bool rotated = false;
        if(Input.GetKeyDown(KeyCode.Q))
        {
            orientationState -= 1;
            rotated = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            orientationState += 1;
            rotated = true;
        }
        if(orientationState < 0)
        {
            orientationState = orientationsDeltas.Count + orientationState;
        }
        else if(orientationState > orientationsDeltas.Count - 1)
        {
            orientationState -= orientationsDeltas.Count;
        }
        if(rotated)
        {
            float startTime = Time.realtimeSinceStartup;
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                if(getColumnIndex(coreTriangle) > gameBoard.getWidth()/ 2 && getColumnIndex(trianglesIndices[i]) < gameBoard.getWidth()/2)
                {
                    coreTriangle -= 4;
                    break;
                }
                else if (getColumnIndex(coreTriangle) < gameBoard.getWidth() / 2 && getColumnIndex(trianglesIndices[i]) > gameBoard.getWidth() / 2)
                {
                    coreTriangle += 4;
                    break;
                }
            }
            updateTriangleIndices();
            Debug.Log(Time.realtimeSinceStartup - startTime);
        }
    }


    void rotateNeo(bool rotateCounterClockwise)
    {
        if(printRotateDebug)
        {
            Debug.Log("Very Start");
            for(int i = 0; i < trianglesIndices.Count; ++i)
            {
                Debug.Log(trianglesIndices[i]);
            }
        }
        int rotateDelta = 0;
        if (rotateCounterClockwise)
        {
            rotateDelta = -1;
        }
        else if (!rotateCounterClockwise)
        {
            rotateDelta = 1;
        }

        bool spinningLeft = false;
        if(spinDirectionCheckFlag)
        {
            if (rotateCounterClockwise)
            {
                spinningLeft = ((orientationState + rotateDelta) % 4 == clockwiseLeft) || ((orientationState + rotateDelta) % 4 == (clockwiseLeft - 1) % 4);
            }
            else
            {
                spinningLeft = ((orientationState + rotateDelta) % 4 == counterwiseLeft) || ((orientationState + rotateDelta) % 4 == (counterwiseLeft + 1) % 4);
            }
        }
        //Debug.Log(rotateCounterClockwise);
        //Debug.Log(orientationState + rotateDelta);
        //Debug.Log(spinningLeft);

        if(rotateDelta != 0)
        {
            float startTime = Time.realtimeSinceStartup;
            int additionalShift = 0;
            List<int> rotatedTriangleIndices =  getNewTriangleIndices(0, 0, rotateDelta);
            //for(int i = 0; i < rotatedTriangleIndices.Count; ++i)
            //{
            //    if(rotatedTriangleIndices[i] < 0)
            //    {
            //        for(int j = 0; j < rotatedTriangleIndices.Count; ++j)
            //        {
            //            rotatedTriangleIndices[j] += 4;
            //        }
            //        --i;
            //    }
            //}
            bool rightSide = getColumnIndex(trianglesIndices[0]) > gameBoard.getWidth() / 2;
            for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
            {
                if (getColumnIndex(rotatedTriangleIndices[0]) > 7 * (gameBoard.getWidth() / 8) && getColumnIndex(rotatedTriangleIndices[i]) < gameBoard.getWidth() / 8)
                {
                    //Debug.Log("Cross Detected A");
                    if (rightSide)
                    {

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            //Debug.Log("Shifting Left");
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                if(getColumnIndex(rotatedTriangleIndices[j]) < gameBoard.getWidth()/2)
                                {
                                    for(int k = 0; k < rotatedTriangleIndices.Count; ++k)
                                    {
                                        //Debug.Log("Before");
                                        //Debug.Log(rotatedTriangleIndices[k]);
                                        rotatedTriangleIndices[k] -= 4;
                                        //Debug.Log("After");
                                        //Debug.Log(rotatedTriangleIndices[k]);
                                    }
                                    additionalShift -= 4;
                                    j = 0;
                                }
                            }
                        }
                        else if (trianglesIndices.Count < 12)
                        {
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                rotatedTriangleIndices[j] -= 4;
                            }
                            additionalShift = -4;
                        }
                    }
                    else
                    {

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            //Debug.Log("Shifting Right");
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                if(getColumnIndex(rotatedTriangleIndices[j]) > gameBoard.getWidth() / 2)
                                {
                                    for(int k = 0; k < rotatedTriangleIndices.Count; ++k)
                                    {
                                        Debug.Log("Before");
                                        Debug.Log(rotatedTriangleIndices[k]);
                                        rotatedTriangleIndices[k] += 4;
                                        Debug.Log("After");
                                        Debug.Log(rotatedTriangleIndices[k]);
                                    }
                                    additionalShift += 4;
                                    --j;
                                }
                            }
                        }
                        else if (trianglesIndices.Count <= 4)
                        {
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                //Debug.Log("Before");
                                //Debug.Log(rotatedTriangleIndices[j]);
                                rotatedTriangleIndices[j] += 4;
                                //Debug.Log("After");
                                //Debug.Log(rotatedTriangleIndices[j]);
                            }
                            additionalShift = 4;
                        }
                    }
                    //break;
                }
                else if (getColumnIndex(rotatedTriangleIndices[0]) < gameBoard.getWidth() / 8 && getColumnIndex(rotatedTriangleIndices[i]) > 7 * (gameBoard.getWidth() / 8))
                {
                    //Debug.Log("Cross Detected B");
                    if (rightSide)
                    {

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                if(getColumnIndex(rotatedTriangleIndices[j]) < gameBoard.getWidth() / 2)
                                {
                                    for(int k = 0; k < rotatedTriangleIndices.Count; ++k)
                                    {
                                        //Debug.Log("Before");
                                        //Debug.Log(rotatedTriangleIndices[k]);
                                        rotatedTriangleIndices[k] -= 4;
                                        //Debug.Log("After");
                                        //Debug.Log(rotatedTriangleIndices[k]);
                                    }
                                    additionalShift -= 4;
                                    --j;
                                }
                            }
                        }
                        else if (trianglesIndices.Count > 4)
                        {
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                //Debug.Log("Before");
                                //Debug.Log(rotatedTriangleIndices[j]);
                                rotatedTriangleIndices[j] -= 4;
                                //Debug.Log("After");
                                //Debug.Log(rotatedTriangleIndices[j]);
                            }
                            additionalShift = -4;
                        }
                    }
                    else
                    {
                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            //Debug.Log("Large");
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                if(getColumnIndex(rotatedTriangleIndices[j]) > gameBoard.getWidth()/2)
                                {
                                    //Debug.Log("New Loop");
                                    //Debug.Log("Shifting Right");
                                    for (int k = 0; k < rotatedTriangleIndices.Count; ++k)
                                    {
                                        //Debug.Log("Before");
                                        //Debug.Log(rotatedTriangleIndices[k]);
                                        rotatedTriangleIndices[k] += 4;
                                        //Debug.Log("After");
                                        //Debug.Log(rotatedTriangleIndices[k]);
                                    }
                                    additionalShift += 4;
                                    --j;
                                }
                            }
                        }
                        else if (trianglesIndices.Count > 4)
                        {
                            for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                            {
                                //Debug.Log("Before");
                                //Debug.Log(rotatedTriangleIndices[j]);
                                rotatedTriangleIndices[j] += 4;
                                //Debug.Log("After");
                                //Debug.Log(rotatedTriangleIndices[j]);
                            }
                            additionalShift = 4;
                        }
                    }
                    //for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                    //{
                    //    rotatedTriangleIndices[j] += 4;
                    //}
                    //if (trianglesIndices.Count >= 12)
                    //{
                    //    additionalShift = 8;
                    //}
                    //else if (trianglesIndices.Count > 4)
                    //{
                    //    additionalShift = 4;
                    //}
                    //else
                    //{
                    //    additionalShift = 8;
                    //}
                    //break;
                }
            }
            //if (printRotateDebug)
            //{
            //    Debug.Log("Pre Downward Shift");
            //    for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
            //    {
            //        Debug.Log(rotatedTriangleIndices[i]);
            //    }
            //}
            int downShift = 0;
            for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
            {
                //rotatedTriangleIndices[i] += additionalShift;
                if (getRowIndex(rotatedTriangleIndices[i]) >= gameBoard.getHeight())
                {
                    for(int j = 0; j < rotatedTriangleIndices.Count; ++j)
                    {
                        rotatedTriangleIndices[j] -= 4 * gameBoard.getWidth();
                    }
                    ++downShift;
                    i = 0;
                }
            }
            if(printRotateDebug)
            {
                Debug.Log("Pre Proximity Check Rotated Triangle indices");
                for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
                {
                    Debug.Log(rotatedTriangleIndices[i]);
                }
            }
            if(rotateCounterClockwise || ((!spinDirectionCheckFlag) || (spinDirectionCheckFlag && !spinningLeft)))
            {
                if (gameBoard.checkEmpty(rotatedTriangleIndices))
                {
                    orientationState += rotateDelta;
                    coreTriangle += additionalShift;
                    if (printRotateDebug)
                    {
                        Debug.Log(downShift);
                        Debug.Log("A");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
                        {
                            Debug.Log(rotatedTriangleIndices[i]);
                        }
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 0)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (4 + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, 0);
                    if (printRotateDebug)
                    {
                        Debug.Log("C");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, 0);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 0)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                    if (printRotateDebug)
                    {
                        Debug.Log("B");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, -1)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 * gameBoard.getWidth() + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 0, -1);
                    if (printRotateDebug)
                    {
                        Debug.Log("D");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, -1);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, -1)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 * gameBoard.getWidth() + 4 + additionalShift);
                    if (printRotateDebug)
                    {
                        Debug.Log("F");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, -1);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, -1)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 * gameBoard.getWidth() - 4 + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, -1);
                    if (printRotateDebug)
                    {
                        Debug.Log("E");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, -1);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, -2)))
                {
                    if (printRotateDebug)
                    {
                        Debug.Log("G");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, -2);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    orientationState += rotateDelta;
                    coreTriangle += (-8 * gameBoard.getWidth() + additionalShift);
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, -2)))
                {
                    if (printRotateDebug)
                    {
                        Debug.Log("I");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, -2);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    orientationState += rotateDelta;
                    coreTriangle += (-8 * gameBoard.getWidth() + 4 + additionalShift);
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, -2)))
                {
                    if (printRotateDebug)
                    {
                        Debug.Log("H");
                        Debug.Log("downShift: " + downShift);
                        Debug.Log("additionalShift: " + additionalShift);
                        Debug.Log("coreTriangle: " + coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, -2);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    orientationState += rotateDelta;
                    coreTriangle += (-8 * gameBoard.getWidth() - 4 + additionalShift);
                }
                else if (rotatedTriangleIndices.Count > 12)
                {
                    if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, -3)))
                    {
                        orientationState += rotateDelta;
                        coreTriangle += (-12 * gameBoard.getWidth() + additionalShift);
                        if (printRotateDebug)
                        {
                            Debug.Log("J");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, -3);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 2, -2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("L");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 2, -2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (-8 * gameBoard.getWidth() + 8 + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -2, -2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("K");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -2, -2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (-8 * gameBoard.getWidth() - 8 + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, 2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("K");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, 2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (8 * gameBoard.getWidth() + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("L");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, 2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (8 * gameBoard.getWidth() + 4 + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("L");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, 2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (8 * gameBoard.getWidth() - 4 + additionalShift);
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, 1)))
                {
                    Debug.Log("M");
                    orientationState += rotateDelta;
                    coreTriangle += (4 * gameBoard.getWidth() + additionalShift);
                    //Debug.Log("G");
                    List<int> check = shiftPassedIndices(rotatedTriangleIndices, 0, 1);
                    for (int i = 0; i < check.Count; ++i)
                    {
                        Debug.Log(check[i]);
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 1)))
                {
                    Debug.Log("O");
                    orientationState += rotateDelta;
                    coreTriangle += (4 * gameBoard.getWidth() + 4 + additionalShift);
                    //Debug.Log("I");
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, 1);
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 1)))
                {
                    Debug.Log("N");
                    orientationState += rotateDelta;
                    coreTriangle += (4 * gameBoard.getWidth() - 4 + additionalShift);
                    //Debug.Log("H");
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 1);
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else
                {
                    return;
                }
            }
            else if (((!spinDirectionCheckFlag) || (spinDirectionCheckFlag && spinningLeft)))
            {
                if (gameBoard.checkEmpty(rotatedTriangleIndices))
                {
                    orientationState += rotateDelta;
                    coreTriangle += additionalShift;
                    if (printRotateDebug)
                    {
                        Debug.Log(downShift);
                        Debug.Log("A");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
                        {
                            Debug.Log(rotatedTriangleIndices[i]);
                        }
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 0)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                    if (printRotateDebug)
                    {
                        Debug.Log("B");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 0)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (4 + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, 0);
                    if (printRotateDebug)
                    {
                        Debug.Log("C");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, 0);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, -1)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 * gameBoard.getWidth() + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 0, -1);
                    if (printRotateDebug)
                    {
                        Debug.Log("D");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, -1);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, -1)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 * gameBoard.getWidth() - 4 + additionalShift);
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, -1);
                    if (printRotateDebug)
                    {
                        Debug.Log("E");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, -1);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, -1)))
                {
                    orientationState += rotateDelta;
                    coreTriangle += (-4 * gameBoard.getWidth() + 4 + additionalShift);
                    if (printRotateDebug)
                    {
                        Debug.Log("F");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, -1);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, -2)))
                {
                    if (printRotateDebug)
                    {
                        Debug.Log("G");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, -2);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    orientationState += rotateDelta;
                    coreTriangle += (-8 * gameBoard.getWidth() + additionalShift);
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, -2)))
                {
                    if (printRotateDebug)
                    {
                        Debug.Log("H");
                        Debug.Log("downShift: " + downShift);
                        Debug.Log("additionalShift: " + additionalShift);
                        Debug.Log("coreTriangle: " + coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, -2);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    orientationState += rotateDelta;
                    coreTriangle += (-8 * gameBoard.getWidth() - 4 + additionalShift);
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, -2)))
                {
                    if (printRotateDebug)
                    {
                        Debug.Log("I");
                        Debug.Log(downShift);
                        Debug.Log(additionalShift);
                        Debug.Log(coreTriangle);
                        Debug.Log("Printing Shifted");
                        List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, -2);
                        for (int i = 0; i < shiftedIndices.Count; ++i)
                        {
                            Debug.Log(shiftedIndices[i] + additionalShift);
                        }
                    }
                    orientationState += rotateDelta;
                    coreTriangle += (-8 * gameBoard.getWidth() + 4 + additionalShift);
                }
                else if (rotatedTriangleIndices.Count > 12)
                {
                    if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, -3)))
                    {
                        orientationState += rotateDelta;
                        coreTriangle += (-12 * gameBoard.getWidth() + additionalShift);
                        if (printRotateDebug)
                        {
                            Debug.Log("J");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, -3);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -2, -2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("K");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -2, -2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (-8 * gameBoard.getWidth() - 8 + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 2, -2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("L");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 2, -2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (-8 * gameBoard.getWidth() + 8 + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, 2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("K");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 0, 2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (8 * gameBoard.getWidth() + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("L");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, -1, 2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (8 * gameBoard.getWidth() - 4 + additionalShift);
                    }
                    else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 2)))
                    {
                        if (printRotateDebug)
                        {
                            Debug.Log("L");
                            Debug.Log(downShift);
                            Debug.Log(additionalShift);
                            Debug.Log(coreTriangle);
                            Debug.Log("Printing Shifted");
                            List<int> shiftedIndices = shiftPassedIndices(rotatedTriangleIndices, 1, 2);
                            for (int i = 0; i < shiftedIndices.Count; ++i)
                            {
                                Debug.Log(shiftedIndices[i] + additionalShift);
                            }
                        }
                        orientationState += rotateDelta;
                        coreTriangle += (8 * gameBoard.getWidth() + 4 + additionalShift);
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, 1)))
                {
                    Debug.Log("M");
                    orientationState += rotateDelta;
                    coreTriangle += (4 * gameBoard.getWidth() + additionalShift);
                    //Debug.Log("G");
                    List<int> check = shiftPassedIndices(rotatedTriangleIndices, 0, 1);
                    for (int i = 0; i < check.Count; ++i)
                    {
                        Debug.Log(check[i]);
                    }
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 1)))
                {
                    Debug.Log("N");
                    orientationState += rotateDelta;
                    coreTriangle += (4 * gameBoard.getWidth() - 4 + additionalShift);
                    //Debug.Log("H");
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 1);
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 1)))
                {
                    Debug.Log("O");
                    orientationState += rotateDelta;
                    coreTriangle += (4 * gameBoard.getWidth() + 4 + additionalShift);
                    //Debug.Log("I");
                    //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, 1);
                    //for (int i = 0; i < check.Count; ++i)
                    //{
                    //    Debug.Log(check[i]);
                    //}
                }
                else
                {
                    return;
                }
            }
            //timer += 0.5f * Time.deltaTime;
            timer += dropTimeIntervalBase/2;
            gameBoard.emptyTriangles(trianglesIndices);
            coreTriangle -= downShift * gameBoard.getWidth() * 4;
            updateTriangleIndices();
            //for(int i = 0; i < trianglesIndices.Count; ++i)
            //{
            //    if(trianglesIndices[i] > gameBoard.getWidth() * gameBoard.height)
            //}
            //int additionalDownShift = 0;
            //for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
            //{
            //    //rotatedTriangleIndices[i] += additionalShift;
            //    if (getRowIndex(rotatedTriangleIndices[i]) >= gameBoard.getHeight() - 1)
            //    {
            //        for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
            //        {
            //            rotatedTriangleIndices[j] -= 4 * gameBoard.getWidth();
            //        }
            //        ++additionalDownShift;
            //    }
            //}
            //coreTriangle -= additionalDownShift * gameBoard.getWidth() * 4;
            gameBoard.dropGhostPiece();
            audioSource.PlayOneShot(slideRotateClip, slideRotateClipVolume);
        }
    }

    void horizontalMove(bool shiftLeft)
    {
        if(shiftLeft)
        {
            for(int i = 0; i < trianglesIndices.Count; ++i)
            {
                if(getColumnIndex(trianglesIndices[i]) == 0)
                {
                    return;
                }
            }
            List<int> leftTriangleIndices = getNewTriangleIndices(-1, 0);
            List<int> leftBelowTriangleIndices = getNewTriangleIndices(-1, -1);
            List<int> leftAboveTriangleIndices = getNewTriangleIndices(-1, 1);


            List<int> leftBelowExtraIndicies = new List<int>();

            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                // Adding additional hitboxes for moving straight to the left
                //if (trianglesIndices[i] % 4 == 1 || trianglesIndices[i] % 4 == 3)
                //{
                //    leftTriangleIndices.Add(trianglesIndices[i] - 2);
                //}
                if (trianglesIndices[i] % 4 == 3)
                {
                    leftTriangleIndices.Add(trianglesIndices[i] - 1);
                    leftTriangleIndices.Add(trianglesIndices[i] - 2);
                    leftTriangleIndices.Add(trianglesIndices[i] - 3);
                    //leftBelowTriangleIndices.Add(trianglesIndices[i] - 1);
                    //leftBelowTriangleIndices.Add(trianglesIndices[i] - 2);
                    leftBelowTriangleIndices.Add(trianglesIndices[i] - 3);
                    leftBelowTriangleIndices.Add(trianglesIndices[i] - 3);
                }


                //if(trianglesIndices[i] % 4 == 1)
                //{
                //    leftAboveTriangleIndices.Add(trianglesIndices[i] - 2);
                //}

                if (leftAboveTriangleIndices[i] % 4 == 1)
                {
                    leftAboveTriangleIndices.Add(leftAboveTriangleIndices[i] + 2);
                }

                //if(leftBelowTriangleIndices[i] % 4 == 3)
                //{
                //    leftBelowExtraIndicies.Add(leftBelowTriangleIndices[i] + 3);
                //}
                //if (leftBelowTriangleIndices[i] % 4 == 0)
                //{
                //    leftBelowExtraIndicies.Add(leftBelowTriangleIndices[i] + 3);
                //}
                //if (trianglesIndices[i] % 4 == 3)
                //{
                //    leftBelowExtraIndicies.Add(trianglesIndices[i] - 3);
                //}
                //else if(trianglesIndices[i] % 4 == 2)
                //{
                //    leftBelowExtraIndicies.Add(trianglesIndices[i] - 1);
                //}
            }

            //for(int i = 0; i < leftBelowExtraIndicies.Count; ++i)
            //{
            //    leftBelowTriangleIndices.Add(leftBelowExtraIndicies[i]);
            //}

            int aboveRow = 0;
            int belowRow = 0;
            bool rowChecked = false;
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                int current = trianglesIndices[i];
                if (i == 0)
                {
                    belowRow = trianglesIndices[0] / 1000;
                    aboveRow = trianglesIndices[0] / 1000;
                    rowChecked = false;
                }
                else
                {
                    if (current /(gameBoard.getWidth() * 4) != belowRow)
                    {
                        belowRow = current / (gameBoard.getWidth() * 4);
                        aboveRow = current / (gameBoard.getWidth() * 4);
                        rowChecked = false;
                    }
                }
                if (current % 4 == 0 && !rowChecked)
                {
                    rowChecked = true;
                    leftAboveTriangleIndices.Add(current + 2);
                }
                else if (current % 4 == 1)
                {
                    leftTriangleIndices.Add(current - 2);
                    //leftBelowTriangleIndices.Add(current - 2);
                }
                //else if (current % 4 == 2 && !rowChecked)
                //{
                //    rowChecked = true;
                //    leftBelowTriangleIndices.Add(current - 2);
                //}
                else if (current % 4 == 3)
                {
                    leftTriangleIndices.Add(current - 2);
                    //leftBelowTriangleIndices.Add(current - 2);
                }
            }
            if(gameBoard.checkEmpty(leftTriangleIndices))
            {
                coreTriangle -= 4;
                //timer += 0.5f * Time.deltaTime;
                //timer += dropTimeIntervalBase / 2;
            }
            else if(gameBoard.checkEmpty(leftBelowTriangleIndices))
            {
                coreTriangle -= (4 + gameBoard.width * 4);
                //timer += 0.5f * Time.deltaTime;
                timer += dropTimeIntervalBase / 2;
            }
            else if (gameBoard.checkEmpty(leftAboveTriangleIndices))
            {
                //if(isTrap)
                //{
                //    List<int> trapHitbox = new List<int>();
                //    for(int i = 0; i < trianglesIndices.Count; ++i)
                //    {
                //        if(trianglesIndices[i] % 4 == 1)
                //        {
                //            trapHitbox.Add(trianglesIndices[i] - 2);
                //            break;
                //        }
                //    }
                //    if(!gameBoard.checkEmpty(trapHitbox))
                //    {
                //        return;
                //    }
                //}
                coreTriangle += ( gameBoard.width * 4 - 4);
                //timer += 0.5f * Time.deltaTime;
                timer += dropTimeIntervalBase / 2;
            }
            else
            {
                return;
            }
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            gameBoard.dropGhostPiece();
            audioSource.PlayOneShot(slideRotateClip, slideRotateClipVolume);
        }
        else if(!shiftLeft)
        {
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                if (getColumnIndex(trianglesIndices[i]) == gameBoard.width - 1)
                {
                    return;
                }
            }
            List<int> rightTriangleIndices = getNewTriangleIndices(1, 0);
            List<int> rightBelowTriangleIndices = getNewTriangleIndices(1, -1);
            List<int> rightAboveTriangleIndices = getNewTriangleIndices(1, 1);


            List<int> rightBelowExtraIndicies = new List<int>();
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                //if (trianglesIndices[i] % 4 == 1 || trianglesIndices[i] % 4 == 3)
                //{
                //    rightTriangleIndices.Add(trianglesIndices[i] + 2);
                //}


                if (trianglesIndices[i] % 4 == 1)
                {
                    rightTriangleIndices.Add(trianglesIndices[i] - 1);
                    rightTriangleIndices.Add(trianglesIndices[i] + 1);
                    rightTriangleIndices.Add(trianglesIndices[i] + 2);
                    rightBelowTriangleIndices.Add(trianglesIndices[i] - 1);
                    //rightBelowTriangleIndices.Add(trianglesIndices[i] + 1);
                    //rightBelowTriangleIndices.Add(trianglesIndices[i] + 2);
                }



                //if (trianglesIndices[i] % 4 == 3)
                //{
                //    rightAboveTriangleIndices.Add(trianglesIndices[i] + 2);
                //}



                if (rightAboveTriangleIndices[i] % 4 == 3)
                {
                    rightAboveTriangleIndices.Add(rightAboveTriangleIndices[i] - 2);
                }

                //if (rightBelowTriangleIndices[i] % 4 == 1)
                //{
                //    rightBelowExtraIndicies.Add(rightBelowTriangleIndices[i] - 3);
                //}
                //if (rightBelowTriangleIndices[i] % 4 == 0)
                //{
                //    rightBelowExtraIndicies.Add(rightBelowTriangleIndices[i] + 1);
                //}

                //if (trianglesIndices[i] % 4 == 1)
                //{
                //    rightBelowExtraIndicies.Add(trianglesIndices[i] - 1);
                //}
                //else if (trianglesIndices[i] % 4 == 2)
                //{
                //    rightBelowExtraIndicies.Add(trianglesIndices[i] + 1);
                //}
            }

            //for (int i = 0; i < rightBelowExtraIndicies.Count; ++i)
            //{
            //    rightBelowTriangleIndices.Add(rightBelowExtraIndicies[i]);
            //}
            int aboveRow = 0;
            int belowRow = 0;
            bool rowChecked = false;
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                int current = trianglesIndices[i];
                //if (current % 4 == 0)
                //{
                //    rightAboveTriangleIndices.Add(current + 2);
                //    rightBelowTriangleIndices.Add(current + 2);
                //}
                if (current % 4 == 1)
                {
                    rightTriangleIndices.Add(current + 2);
                    //rightBelowTriangleIndices.Add(current + 2);
                }
                //else if (current % 4 == 2)
                //{
                //    rightBelowTriangleIndices.Add(current - 2);
                //}
                else if (current % 4 == 3)
                {
                    rightTriangleIndices.Add(current + 2);
                    //rightBelowTriangleIndices.Add(current + 2);
                }
            }
            for (int i = trianglesIndices.Count - 1; i >= 0; --i)
            {
                int current = trianglesIndices[i];
                if (i == trianglesIndices.Count - 1)
                {
                    belowRow = trianglesIndices[trianglesIndices.Count - 1] / 1000;
                    aboveRow = trianglesIndices[trianglesIndices.Count - 1] / 1000;
                    rowChecked = false;
                }
                else
                {
                    if (current / (gameBoard.getWidth() * 4) != belowRow)
                    {
                        belowRow = current / (gameBoard.getWidth() * 4);
                        aboveRow = current / (gameBoard.getWidth() * 4);
                        rowChecked = false;
                    }
                }
                if (current % 4 == 0 && !rowChecked)
                {
                    rightAboveTriangleIndices.Add(current + 2);
                    rowChecked = true;
                    //rightBelowTriangleIndices.Add(current + 2);
                }
                //else if (current % 4 == 1)
                //{
                //    rightTriangleIndices.Add(current + 2);
                //    //rightBelowTriangleIndices.Add(current + 2);
                //}
                //else if (current % 4 == 2 && !rowChecked)
                //{
                //    rightBelowTriangleIndices.Add(current - 2);
                //    rowChecked = true;
                //}
                //else if (current % 4 == 3)
                //{
                //    rightTriangleIndices.Add(current + 2);
                //    //rightBelowTriangleIndices.Add(current + 2);
                //}
            }
            if (gameBoard.checkEmpty(rightTriangleIndices))
            {
                coreTriangle += 4;
                //timer += 0.5f * Time.deltaTime;
                //timer += dropTimeIntervalBase / 2;
            }
            else if (gameBoard.checkEmpty(rightBelowTriangleIndices))
            {
                coreTriangle -= (gameBoard.width * 4 - 4);
                //timer += 0.5f * Time.deltaTime;
                timer += dropTimeIntervalBase / 2;
            }
            else if (gameBoard.checkEmpty(rightAboveTriangleIndices))
            {
                //if (isTrap)
                //{
                //    List<int> trapHitbox = new List<int>();
                //    for (int i = 0; i < trianglesIndices.Count; ++i)
                //    {
                //        if (trianglesIndices[i] % 4 == 3)
                //        {
                //            trapHitbox.Add(trianglesIndices[i] + 2);
                //            break;
                //        }
                //    }
                //    if (!gameBoard.checkEmpty(trapHitbox))
                //    {
                //        return;
                //    }
                //}
                coreTriangle += (gameBoard.width * 4 + 4);
                //timer += 0.5f * Time.deltaTime;
                timer += dropTimeIntervalBase / 2;
            }
            else
            {
                return;
            }
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            gameBoard.dropGhostPiece();
            audioSource.PlayOneShot(slideRotateClip, slideRotateClipVolume);
        }
    }

    void lower()
    {
        if (!gameBoard.checkEmpty(getNewTriangleIndices(0, -1)))
        {
            return;
        }
        fallOnce();
        timer = dropTimeIntervalBase;
        //if(Input.GetKey(controlsKeys[softDropIndex]) && !letter)
        //{
        //    Debug.Log("Playing lower sound");
        //    audioSource.PlayOneShot(slideRotateClip, 0.5f);
        //}
    }

    public void drop()
    {
        //for (int i = 0; i < gameBoard.height; ++i)
        //{
        //    fallOnce();
        //}
        if(gameBoard.checkEmpty(getNewTriangleIndices(0, -1)))
        {
            gameBoard.emptyTriangles(trianglesIndices);
            coreTriangle = gameBoard.GetComponent<Board>().getGhostCoreTriangle();
            updateTriangleIndices();
            if(lockDelayFlag)
            {
                timer = dropTimeIntervalBase;
            }
            else
            {
                timer = 0;
            }
            //audioSource.PlayOneShot(hardDropClip);
            AudioSource.PlayClipAtPoint(hardDropClip, new Vector3(0, 0, 0), hardDropClipVolume);
            //AudioSource.PlayClipAtPoint()
        }
    }

    public void ghostDrop()
    {
        List<int> shadow = new List<int>(trianglesIndices);
        List<int> shadowHitbox = new List<int>(trianglesIndices);

        for(int i = 0; i < shadowHitbox.Count; ++i)
        {
            shadowHitbox[i] -= gameBoard.getWidth() * 4;
        }
        for (int i = 0; i < shadow.Count; ++i)
        {
            if(shadow[i] % 4 == 0)
            {
                shadowHitbox.Add(shadow[i] - gameBoard.getWidth() * 4 + 2);
            }
            else if (shadow[i] % 4 == 2)
            {
                shadowHitbox.Add(shadow[i] - 2);
            }
        }

        for (int i = 0; i < gameBoard.getHeight(); ++i)
        {
            if(gameBoard.checkEmpty(shadowHitbox))
            {
                coreTriangle -= gameBoard.getWidth() * 4;
                for (int j = 0; j < shadowHitbox.Count; ++j)
                {
                    shadowHitbox[j] -= gameBoard.getWidth() * 4;
                }
                for (int j = 0; j < shadow.Count; ++j)
                {
                    shadow[j] -= gameBoard.getWidth() * 4;
                }
            }
            else
            {
                trianglesIndices = shadow;
                gameBoard.updateBoard(trianglesIndices, pieceColor);
                return;
            }
        }
    }

    public void toggleActive(bool toggle)
    {
        active = toggle;
    }

    List<int> shiftPassedIndices(List<int> indices, int newX, int newY)
    {
        List<int> newList = new List<int>(indices);
        for(int i = 0; i < indices.Count; ++i)
        {
            newList[i] += (gameBoard.getWidth() * 4 * newY) + (newX * 4);
        }
        return newList;
    }



    
	// Use this for initialization
	void Start ()
    {
        if (active)
        {
            AudioSource.PlayClipAtPoint(spawnClip, new Vector3(0, 0, 0), spawnClipVolume);
            if (!letter)
            {
                dropTimeIntervalBase = gameBoard.getPieceLowerTimer();
                dropTimeInterval = gameBoard.getPieceLowerTimer();
            }
            timer = dropTimeInterval;
            for (int i = 0; i < orientationsDeltas.Count; ++i)
            {
                for (int j = 0; j < orientationsDeltas[i].myList.Count; ++j)
                {
                    int value = orientationsDeltas[i].myList[j];
                    if (Mathf.Abs(value) > 999)
                    {
                        orientationsDeltas[i].myList[j] = (Mathf.Abs(value) % 1000 / 100 == 1 ? -1 : 1) * (Mathf.Abs(value) % 100) + value / 1000 * gameBoard.width * 4;
                    }
                }
            }
            if (!letter)
            {
                resetPosition();
            }

            updateTriangleIndices();
            gameBoard.dropGhostPiece();
            gameBoard.updateBoard(trianglesIndices, pieceColor);

            audioSource = gameObject.GetComponent<AudioSource>();
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (active && ready)
        {
            if(!suspend || (suspend && Input.GetKey(controlsKeys[softDropIndex])))
            {
                //timer -= Time.deltaTime;
                timer -= 1;
            }
            //rotate();
            if(Input.GetKeyDown(controlsKeys[rotateLeftIndex]))
            {
                rotateNeo(true);
            }
            else if(Input.GetKeyDown(controlsKeys[rotateRightIndex]))
            {
                rotateNeo(false);
            }
            if(Input.GetKeyDown(controlsKeys[shiftLeftIndex]))
            {
                horizontalMove(true);
            }
            else if(Input.GetKeyDown(controlsKeys[shiftRightIndex]))
            {
                horizontalMove(false);
            }

            if (Input.GetKey(controlsKeys[shiftLeftIndex]))
            {
                ++frameCounter;
                //Debug.Log(frameCounter);
                if (firstDelayflag)
                {
                    if (frameCounter >= leftDelayedAutoshift)
                    {
                        horizontalMove(true);
                        frameCounter = 0;
                        firstDelayflag = false;
                    }
                }
                else
                {
                    if (frameCounter >= leftAutoRepeatRate)
                    {
                        horizontalMove(true);
                        frameCounter = 0;
                    }
                }
            }
            else if (Input.GetKey(controlsKeys[shiftRightIndex]))
            {
                ++frameCounter;
                //Debug.Log(frameCounter);
                if (firstDelayflag)
                {
                    if (frameCounter >= rightDelayedAutoshift)
                    {
                        horizontalMove(false);
                        frameCounter = 0;
                        firstDelayflag = false;
                    }
                }
                else
                {
                    if (frameCounter >= rightAutoRepeatRate)
                    {
                        horizontalMove(false);
                        frameCounter = 0;
                    }
                }
            }


            if (Input.GetKeyUp(controlsKeys[shiftLeftIndex]) || Input.GetKeyUp(controlsKeys[shiftRightIndex]))
            {
                frameCounter = -1;
                firstDelayflag = true;
            }

            //if (Input.GetKey(KeyCode.DownArrow))
            //{
            //    dropTimeInterval = dropTimeIntervalBase / 8;
            //    if(timer > dropTimeIntervalBase / 8)
            //    {
            //        timer = dropTimeIntervalBase / 8;
            //    }
            //}
            //else if(!Input.GetKey(KeyCode.DownArrow))
            //{
            //    dropTimeInterval = dropTimeIntervalBase;
            //}
            if(Input.GetKeyDown(controlsKeys[softDropIndex]))
            {
                lower();
            }


            if (Input.GetKey(controlsKeys[softDropIndex]))
            {
                dropTimeInterval = dropTimeIntervalBase / 8;
                if (timer > dropTimeIntervalBase / 8)
                {
                    timer = dropTimeIntervalBase / 8;
                }
            }
            else if (Input.GetKeyUp(controlsKeys[softDropIndex]))
            {
                dropTimeInterval = dropTimeIntervalBase;
                timer = dropTimeIntervalBase;
            }

            if(Input.GetKeyDown(controlsKeys[lockInIndex]) && !gameBoard.checkEmpty(shiftPassedIndices(trianglesIndices, 0, -1)))
            {
                timer = 0;
            }

            //if(Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    lower();
            //}

            if (Input.GetKeyDown(controlsKeys[hardDropIndex]))
            {
                drop();
            }
            if (timer <= 0)
            {
                fallOnce();
                timer = dropTimeInterval;
            }
            try
            {
                gameBoard.updateBoard(trianglesIndices, pieceColor);
            }
            catch
            {
                //for(int i = 0; i < trianglesIndices.Count; ++i)
                //{
                //    Debug.Log(trianglesIndices[i]);
                //}
            }
        }
        ready = active;
        if(!active)
        {
            dropTimeInterval = dropTimeIntervalBase;
        }
    }

    //private void OnDestroy()
    //{
    //    gameBoard.clearFilledRows();
    //}
}
