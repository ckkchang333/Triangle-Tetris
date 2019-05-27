using System.Collections;
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


    void fallOnce()
    {
        //for (int i = 0; i < trianglesIndices.Count; ++i)
        //{
        //    trianglesIndices[i] -= gameBoard.length * 4;
        //}
        coreTriangle -= gameBoard.length * 4;
        updateTriangleIndices();
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
        if(Input.GetKeyDown(KeyCode.Q))
        {
            orientationState -= 1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            orientationState += 1;
        }
        if(orientationState < 0)
        {
            orientationState = 4 + orientationState;
        }
        else if(orientationState > 3)
        {
            orientationState -= 4;
        }
        gameBoard.emptyTriangles(trianglesIndices);
        updateTriangleIndices();
        gameBoard.updateBoard(trianglesIndices, pieceColor);
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
                    orientationsDeltas[i].myList[j] = Mathf.Abs(value) % 1000 + value / 1000 * gameBoard.length * 4;
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
        if (timer <= 0)
        {
            gameBoard.emptyTriangles(trianglesIndices);
            if (gameBoard.checkEmptyBelow(trianglesIndices))
            {

                fallOnce();
            }
            else
            {
                Destroy(this.gameObject);
            }
            gameBoard.updateBoard(trianglesIndices, pieceColor);
            timer = dropTimeInterval;
        }

    }
}
