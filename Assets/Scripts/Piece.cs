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
    public int pieceID;
    public bool active;

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
                    fallTriangleIndices.Insert(i, oppositeBelowTriangleIndex);
                    ++i;
                }
            }
            else if (current % 4 == 2)
            {
                int oppositeBelowTriangleIndex = current - 2;
                if (!fallTriangleIndices.Contains(oppositeBelowTriangleIndex) && oppositeBelowTriangleIndex >= 0)
                {
                    fallTriangleIndices.Insert(i, oppositeBelowTriangleIndex);
                    ++i;
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
        coreTriangle = (gameBoard.getHeight() - 3) * (gameBoard.getWidth() * 4) + (gameBoard.getWidth() / 2) * 4;
    }

    List<int> getNewTriangleIndices(int newX, int newY)
    {
        List<int> newTriangleIndices = new List<int>();
        int newCoreTriangle = coreTriangle - gameBoard.width * 4 * -newY;
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
                //if ((getRowIndex(trianglesIndices[i]) > getRowIndex(coreTriangle) && getColumnIndex(trianglesIndices[i]) == 0 && getColumnIndex(coreTriangle) != 0)
                //   ||  (getColumnIndex(trianglesIndices[i]) == 0 && getColumnIndex(coreTriangle) != 0))
                if(getColumnIndex(coreTriangle) > gameBoard.getWidth()/ 2 && getColumnIndex(trianglesIndices[i]) < gameBoard.getWidth()/2)
                {
                    coreTriangle -= 4;
                    break;
                }
                //else if ((getRowIndex(trianglesIndices[i]) < getRowIndex(coreTriangle) && getColumnIndex(trianglesIndices[i]) == gameBoard.width - 1 && getColumnIndex(coreTriangle) != gameBoard.width - 1)
                //        || (getColumnIndex(trianglesIndices[i]) == gameBoard.width - 1 && getColumnIndex(coreTriangle) != gameBoard.width - 1))
                else if (getColumnIndex(coreTriangle) < gameBoard.getWidth() / 2 && getColumnIndex(trianglesIndices[i]) > gameBoard.getWidth() / 2)
                {
                    coreTriangle += 4;
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
            List<int> leftTriangleIndices = getNewTriangleIndices(-1, 0);
            for(int i = 0; i < leftTriangleIndices.Count; ++i)
            {
                int current = leftTriangleIndices[i];
                if(current % 4 == 1)
                {
                    leftTriangleIndices.Insert(i, current + 2);
                    ++i;
                }
                else if (current % 4 == 3)
                {
                    leftTriangleIndices.Insert(i, current + 2);
                    ++i;
                }
            }
            if(gameBoard.checkEmpty(leftTriangleIndices))
            {
                coreTriangle -= 4;
            }
            else if(gameBoard.checkEmpty(getNewTriangleIndices(-1, -1)))
            {
                coreTriangle -= (4 + gameBoard.width * 4);
            }
            else if (gameBoard.checkEmpty(getNewTriangleIndices(-1, 1)))
            {
                coreTriangle += ( gameBoard.width * 4 - 4);
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
                if (getColumnIndex(trianglesIndices[i]) == gameBoard.width - 1)
                {
                    return;
                }
            }
            List<int> rightTriangleIndices = getNewTriangleIndices(1, 0);
            for (int i = 0; i < rightTriangleIndices.Count; ++i)
            {
                int current = rightTriangleIndices[i];
                if (current % 4 == 1)
                {
                    rightTriangleIndices.Insert(i, current - 2);
                    ++i;
                }
                else if (current % 4 == 3)
                {
                    rightTriangleIndices.Insert(i, current - 2);
                    ++i;
                }
            }
            if (gameBoard.checkEmpty(rightTriangleIndices))
            {
                coreTriangle += 4;
            }
            else if (gameBoard.checkEmpty(getNewTriangleIndices(1, -1)))
            {
                coreTriangle -= (gameBoard.width * 4 - 4);
            }
            else if (gameBoard.checkEmpty(getNewTriangleIndices(1, 1)))
            {
                coreTriangle += (gameBoard.width * 4 + 4);
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
            coreTriangle -= 4 * gameBoard.width;
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

    public void activate(bool toggle)
    {
        active = toggle;
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
                    orientationsDeltas[i].myList[j] = (Mathf.Abs(value) % 1000 / 100 == 1 ? -1 : 1) *  (Mathf.Abs(value) % 100) + value / 1000 * gameBoard.width * 4;
                }
            }
        }
        resetPosition();
        updateTriangleIndices();

        gameBoard.updateBoard(trianglesIndices, pieceColor);
    }
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
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

    private void OnDestroy()
    {

        gameBoard.clearFilledRows();
    }
}
