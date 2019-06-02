﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ListWrapper
{

    public List<int> myList;
}

public class Piece : MonoBehaviour {

    public Board gameBoard;

    [Header("Piece Parameters")]
    public int coreTriangle;
    public int orientationState;
    public List<int> trianglesIndices;
    public List<ListWrapper> orientationsDeltas;
    public float dropTimeInterval;
    private float timer;

    public Color pieceColor = Color.red;

    int getColumnIndex(int triangleIndex)
    {
        return (triangleIndex % (gameBoard.length * 4)) / 4;
    }


    int getRowIndex(int triangleIndex)
    {
        return triangleIndex / (gameBoard.length * 4);
    }
    void fallOnce()
    {
        List<int> newPosition = getNewTriangleIndices(0, -1);
        for(int i = 0; i < newPosition.Count; ++i)
        {
            Debug.Log(newPosition[i]);
        }
        Debug.Log(gameBoard.checkEmpty(getNewTriangleIndices(0, -1)));
        if(gameBoard.checkEmpty(getNewTriangleIndices(0, -1)))
        {
            gameBoard.emptyTriangles(trianglesIndices);
            coreTriangle -= gameBoard.length * 4;
            updateTriangleIndices();
        }
        else
        {
            gameBoard.setPerm(trianglesIndices);
            Destroy(this.gameObject);
        }
    }

    List<int> getNewTriangleIndices(int newX, int newY)
    {
        List<int> newTriangleIndices = new List<int>();
        int newCoreTriangle = coreTriangle - gameBoard.length * 4 * -newY;
        newCoreTriangle += 4 * newX;
        List<int> deltas = orientationsDeltas[orientationState].myList;
        for (int i = 0; i < deltas.Count; ++i)
        {
            newTriangleIndices.Add(newCoreTriangle + deltas[i]);
        }
        return newTriangleIndices;
    }

    void updateTriangleIndices()
    {
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
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                if (getRowIndex(trianglesIndices[i]) > getRowIndex(coreTriangle) && getColumnIndex(trianglesIndices[i]) == 0 && getColumnIndex(coreTriangle) != 0)
                {
                    coreTriangle -= 4;
                    Debug.Log("Fix Move left");
                    break;
                }
                else if (getRowIndex(trianglesIndices[i]) < getRowIndex(coreTriangle) && getColumnIndex(trianglesIndices[i]) == gameBoard.length - 1 && getColumnIndex(coreTriangle) != gameBoard.length - 1)
                {
                    coreTriangle += 4;
                    Debug.Log("Fix Move right");
                    break;
                }
            }
            updateTriangleIndices();
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
            if(gameBoard.checkEmpty(getNewTriangleIndices(-1, 0)))
            {
                coreTriangle -= 4;
            }
            else if(gameBoard.checkEmpty(getNewTriangleIndices(-1, -1)))
            {
                coreTriangle -= (4 + gameBoard.length * 4);
            }
            else
            {
                return;
            }
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            for (int i = 0; i < trianglesIndices.Count; ++i)
            {
                if (getColumnIndex(trianglesIndices[i]) == gameBoard.length - 1)
                {
                    return;
                }
            }
            if (gameBoard.checkEmpty(getNewTriangleIndices(1, 0)))
            {
                coreTriangle += 4;
            }
            else if (gameBoard.checkEmpty(getNewTriangleIndices(1, -1)))
            {
                coreTriangle -= (gameBoard.length * 4 - 4);
            }
            else
            {
                return;
            }
            gameBoard.emptyTriangles(trianglesIndices);
            updateTriangleIndices();
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
            gameBoard.emptyTriangles(trianglesIndices);
            coreTriangle -= 4 * gameBoard.length;
            updateTriangleIndices();
            timer = dropTimeInterval;
        }
    }

    void drop()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < gameBoard.height; ++i)
            {
                fallOnce();
            }
        }
    }
    
	// Use this for initialization
	void Start () {
        timer = dropTimeInterval;
        for(int i = 0; i < orientationsDeltas.Count; ++i)
        {
            for(int j = 0; j < orientationsDeltas[i].myList.Count; ++j)
            {
                int value = orientationsDeltas[i].myList[j];
                if (Mathf.Abs(value) > 999)
                {
                    orientationsDeltas[i].myList[j] = (Mathf.Abs(value) % 1000 / 100 == 1 ? -1 : 1) *  (Mathf.Abs(value) % 100) + value / 1000 * gameBoard.length * 4;
                }
            }
        }
        updateTriangleIndices();

        gameBoard.updateBoard(trianglesIndices, pieceColor);
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        rotate();
        horizontalMove();
        lower();
        drop();
        if (timer <= 0)
        {
            fallOnce();
            timer = dropTimeInterval;
        }
        gameBoard.updateBoard(trianglesIndices, pieceColor);

    }
}
