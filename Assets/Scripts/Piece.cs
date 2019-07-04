﻿using System.Collections;
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
    public float dropTimeInterval;
    private float timer;
    public int pieceID;
    public bool active;
    public bool letter;
    public bool obilteration;



    public GameObject pieceSprite;

    public Color pieceColor = Color.red;


    public int getPieceID()
    {
        return pieceID;
    }

    public List<int> getTriangleIndices()
    {
        return new List<int>(trianglesIndices);
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
            gameBoard.setPerm(trianglesIndices);
            Destroy(this.gameObject);
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


    void rotateNeo()
    {
        int rotateDelta = 0;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotateDelta = -1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotateDelta = 1;
        }
        if(rotateDelta != 0)
        {
            float startTime = Time.realtimeSinceStartup;
            int additionalShift = 0;
            List<int> rotatedTriangleIndices =  getNewTriangleIndices(0, 0, rotateDelta);
            bool rightSide = false;
            if(getColumnIndex(trianglesIndices[0]) > gameBoard.getWidth() / 2)
            {
                rightSide = true;
            }
            for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
            {
                if (getColumnIndex(coreTriangle) > gameBoard.getWidth() / 2 && getColumnIndex(rotatedTriangleIndices[i]) < gameBoard.getWidth() / 2)
                {
                    if(rightSide)
                    {
                        for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                        {
                            rotatedTriangleIndices[j] -= 4;
                        }

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            additionalShift = -8;
                        }
                        else if (trianglesIndices.Count > 4)
                        {
                            additionalShift = -4;
                        }
                        else
                        {
                            additionalShift = -4;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                        {
                            rotatedTriangleIndices[j] += 4;
                        }

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            additionalShift = 8;
                        }
                        else if (trianglesIndices.Count > 4)
                        {
                            additionalShift = 4;
                        }
                        else
                        {
                            additionalShift = 4;
                        }
                    }
                    break;
                }
                else if (getColumnIndex(coreTriangle) < gameBoard.getWidth() / 2 && getColumnIndex(rotatedTriangleIndices[i]) > gameBoard.getWidth() / 2)
                {
                    if (rightSide)
                    {
                        for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                        {
                            rotatedTriangleIndices[j] -= 4;
                        }

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            additionalShift = -8;
                        }
                        else if (trianglesIndices.Count > 4)
                        {
                            additionalShift = -4;
                        }
                        else
                        {
                            additionalShift = -4;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < rotatedTriangleIndices.Count; ++j)
                        {
                            rotatedTriangleIndices[j] += 4;
                        }

                        if (trianglesIndices.Count >= 12 && !letter)
                        {
                            additionalShift = 8;
                        }
                        else if (trianglesIndices.Count > 4)
                        {
                            additionalShift = 4;
                        }
                        else
                        {
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
                    break;
                }
            }
            int downShift = 0;
            for (int i = 0; i < rotatedTriangleIndices.Count; ++i)
            {
                if (getRowIndex(rotatedTriangleIndices[i]) >= gameBoard.getHeight())
                {
                    for(int j = 0; j < rotatedTriangleIndices.Count; ++j)
                    {
                        rotatedTriangleIndices[j] -= 4 * gameBoard.getWidth();
                    }
                    ++downShift;
                }
            }
            if (gameBoard.checkEmpty(rotatedTriangleIndices))
            {
                orientationState += rotateDelta;
                coreTriangle += additionalShift - (downShift * gameBoard.getWidth() * 4);
                //Debug.Log("A");
                //for(int i = 0; i < rotatedTriangleIndices.Count; ++i)
                //{
                //    Debug.Log(rotatedTriangleIndices[i]);
                //}
            }
            else if(gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 0)))
            {
                orientationState += rotateDelta;
                coreTriangle += (-4 + additionalShift);
                //List<int> check = shiftPassedIndices(rotatedTriangleIndices, -1, 0);
                //Debug.Log("B");
                //for (int i = 0; i < check.Count; ++i)
                //{
                //    Debug.Log(check[i]);
                //}
            }
            else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, 0)))
            {
                orientationState += rotateDelta;
                coreTriangle += (4 + additionalShift);
                //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, 0);
                //Debug.Log("C");
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
                //Debug.Log("D");
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
                //Debug.Log("E");
                //for (int i = 0; i < check.Count; ++i)
                //{
                //    Debug.Log(check[i]);
                //}
            }
            else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 1, -1)))
            {
                orientationState += rotateDelta;
                coreTriangle += (-4 * gameBoard.getWidth() + 4 + additionalShift);
                //Debug.Log("F");
                //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, -1);
                //for (int i = 0; i < check.Count; ++i)
                //{
                //    Debug.Log(check[i]);
                //}
            }
            //else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -2, -1)))
            //{
            //    Debug.Log("D");
            //    orientationState += rotateDelta;
            //    coreTriangle += (-4 * gameBoard.getWidth() - 4 + additionalShift);
            //}
            //else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 2, -1)))
            //{
            //    Debug.Log("E");
            //    orientationState += rotateDelta;
            //    coreTriangle += (-4 * gameBoard.getWidth() + 4 + additionalShift);
            //}
            else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, 0, 1)))
            {
                orientationState += rotateDelta;
                coreTriangle += (4 * gameBoard.getWidth() + additionalShift);
                //Debug.Log("G");
                //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 0, 1);
                //for (int i = 0; i < check.Count; ++i)
                //{
                //    Debug.Log(check[i]);
                //}
            }
            else if (gameBoard.checkEmpty(shiftPassedIndices(rotatedTriangleIndices, -1, 1)))
            {
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
                orientationState += rotateDelta;
                coreTriangle += (4 * gameBoard.getWidth() + 4 + additionalShift);
                //Debug.Log("I");
                //List<int> check = shiftPassedIndices(rotatedTriangleIndices, 1, 1);
                //for (int i = 0; i < check.Count; ++i)
                //{
                //    Debug.Log(check[i]);
                //}
            }
            timer += 0.5f * Time.deltaTime;
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            //for(int i = 0; i < trianglesIndices.Count; ++i)
            //{
            //    if(trianglesIndices[i] > gameBoard.getWidth() * gameBoard.height)
            //}
            gameBoard.dropGhostPiece();
        }
    }

    void horizontalMove()
    {
        if(Input.GetKeyDown(KeyCode.A))
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
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                int current = trianglesIndices[i];
                if(current % 4 == 1)
                {
                    leftTriangleIndices.Add(current - 2);
                    //leftBelowTriangleIndices.Add(current - 2);
                }
                else if (current % 4 == 3)
                {
                    leftTriangleIndices.Add(current - 2);
                    //leftBelowTriangleIndices.Add(current - 2);
                }
            }
            if(gameBoard.checkEmpty(leftTriangleIndices))
            {
                coreTriangle -= 4;
                timer += 0.5f * Time.deltaTime;
            }
            else if(gameBoard.checkEmpty(leftBelowTriangleIndices))
            {
                coreTriangle -= (4 + gameBoard.width * 4);
                timer += 0.5f * Time.deltaTime;
            }
            else if (gameBoard.checkEmpty(getNewTriangleIndices(-1, 1)))
            {
                coreTriangle += ( gameBoard.width * 4 - 4);
                timer += 0.5f * Time.deltaTime;
            }
            else
            {
                return;
            }
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            gameBoard.dropGhostPiece();
        }
        else if(Input.GetKeyDown(KeyCode.D))
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
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                int current = trianglesIndices[i];
                if (current % 4 == 1)
                {
                    rightTriangleIndices.Add(current + 2);
                    //rightBelowTriangleIndices.Add(current + 2);
                }
                else if (current % 4 == 3)
                {
                    rightTriangleIndices.Add(current + 2);
                    //rightBelowTriangleIndices.Add(current + 2);
                }
            }
            if (gameBoard.checkEmpty(rightTriangleIndices))
            {
                coreTriangle += 4;
                timer += 0.5f * Time.deltaTime;
            }
            else if (gameBoard.checkEmpty(rightBelowTriangleIndices))
            {
                coreTriangle -= (gameBoard.width * 4 - 4);
                timer += 0.5f * Time.deltaTime;
            }
            else if (gameBoard.checkEmpty(getNewTriangleIndices(1, 1)))
            {
                coreTriangle += (gameBoard.width * 4 + 4);
                timer += 0.5f * Time.deltaTime;
            }
            else
            {
                return;
            }
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            gameBoard.dropGhostPiece();
        }
    }

    void lower()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!gameBoard.checkEmpty(getNewTriangleIndices(0, -1)))
            {
                return;
            }
            fallOnce();
            timer = dropTimeInterval;
        }
    }

    public void drop()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < gameBoard.height; ++i)
            {
                fallOnce();
            }
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
            if(!letter)
            {
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
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            timer -= Time.deltaTime;
            //rotate();
            rotateNeo();
            horizontalMove();
            lower();
            drop();
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
                for(int i = 0; i < trianglesIndices.Count; ++i)
                {
                    Debug.Log(trianglesIndices[i]);
                }
            }
        }
    }

    private void OnDestroy()
    {
        gameBoard.clearFilledRows();
    }
}
