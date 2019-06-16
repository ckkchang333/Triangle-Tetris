using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    [Header("Board Parameters")]
    public int width = 6;              // in number of grids
    public int height = 12;             // in number of grids
    public float originX = -3.0f;
    public float originY = -4.0f;
    public bool ghostOn = true;

    [Header("Grid Parameters")]
    public GameObject grid;
    public GameObject blockRow;
    public float gridWidth = 1.0f;
    public float gridHeight = 1.0f;
    public float gridSideThickness = 1.0f;
    public float gridCrossingThickness = 0.02f;

    [Header("Piece")]
    public GameObject piecePrefab;
    public GameObject currentPiece;
    public GameObject ghostPiece;


    [Header("External Components")]
    public GameObject pieceQueue;
    public GameObject pieceHolder;
    public Text scoreText;
    public Text gameOverText;


    public int rowsCleared = 0;

    public bool swapLock = false;

    public bool gameOver = false;


    int getColumnIndex(int triangleIndex)
    {
        return (triangleIndex % (getWidth() * 4)) / 4;
    }


    int getRowIndex(int triangleIndex)
    {
        return triangleIndex / (getWidth() * 4);
    }

    void generateBoard ()
    {
        for (int i = 0; i < height; ++i)
        {
            GameObject gridRow = Instantiate(blockRow, new Vector3(originX + gridWidth / 2, originY + i * (gridHeight - gridSideThickness / 2), 0), Quaternion.Euler(0, 0, 0));
            gridRow.transform.parent = this.transform;
            for (int j = 0; j < width; ++j)
            {
                GameObject newBlock = Instantiate(grid, new Vector3(originX + gridWidth / 2 + j * (gridWidth - gridSideThickness / 2), originY + i * (gridHeight - gridSideThickness / 2), 0), Quaternion.Euler(0, 0, 0));
                Block newBlockScript = newBlock.GetComponent<Block>();
                newBlockScript.setSize(gridWidth, gridHeight, gridSideThickness, gridCrossingThickness);
                //newBlockScript.width = gridWidth;
                //newBlockScript.height = gridHeight;
                //newBlockScript.thickness = gridThickness;
                newBlock.transform.parent = gridRow.transform;
                newBlock.gameObject.SetActive(true);
            }
        }
    }

    void generatePiece()
    {
        currentPiece = Instantiate(piecePrefab);
        currentPiece.SetActive(true);
        currentPiece.GetComponent<Piece>().gameBoard = this;
    }
    
    public void emptyTriangles(List<int> triangleIndices)
    {
        for(int i = 0; i < triangleIndices.Count; ++i)
        {
            Block currentBlockScript =  fetchBlockScriptByIndex(triangleIndices[i]);
            currentBlockScript.setQuadFilled((triangleIndices[i] % (width * 4) % 4), false, Color.clear);
        }
    }

    public void updateBoard(List<int> currentTriangleIndices, Color pieceColor)
    {
        // Fill in board according to the triangles that the piece occupies
        Piece pieceScript = currentPiece.GetComponent<Piece>();
        for (int i = 0; i < pieceScript.trianglesIndices.Count; ++i)
        {
            Block currentBlockScript = fetchBlockScriptByIndex(currentTriangleIndices[i]);
            int quadIndex = (currentTriangleIndices[i] % (width * 4) % 4);
            currentBlockScript.setQuadFilled(quadIndex, true, pieceColor);
        }
    }

    public void setGameOver(bool toggle)
    {
        gameOver = toggle;
        gameOverText.gameObject.SetActive(toggle);
    }

    public void resetGame()
    {
        rowsCleared = 0;
        scoreText.text = "Rows Cleared: " + rowsCleared;
        obilterateBoard();
        setGameOver(false);
        return;
    }

    public bool checkEmpty(List<int> triangleIndices)
    {
        for (int i = 0; i < triangleIndices.Count; ++i)
        {
            if(triangleIndices[i] < 0)
            {
                return false;
            }
            Block currentBlockScript = fetchBlockScriptByIndex(triangleIndices[i]);
            int quadrantIndex = (triangleIndices[i] % (width * 4) % 4);
            if ((quadrantIndex == 0 && currentBlockScript.permBottom) ||
                (quadrantIndex == 1 && currentBlockScript.permLeft) ||
                (quadrantIndex == 2 && currentBlockScript.permTop) ||
                (quadrantIndex == 3 && currentBlockScript.permRight))
            {
                return false;
            }
            if(getColumnIndex(triangleIndices[0]) < getWidth()/4)
            {
                if(getColumnIndex(triangleIndices[i]) > 3 * getWidth()/4)
                {
                    return false;
                }
            }
            else if (getColumnIndex(triangleIndices[0]) > 3 * getWidth() / 4)
            {
                if (getColumnIndex(triangleIndices[i]) < getWidth() / 4)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void obilterateBoard()
    {
        for(int i = 0; i < this.getHeight(); ++i)
        {
            for(int j = 0; j < this.getWidth(); ++j)
            {
                Block blockScript = fetchBlockScriptByIndex(i * this.getWidth() * 4 + j * 4);
                if(blockScript.permBottom)
                {
                    blockScript.permBottom = false;
                    blockScript.filledBottom = false;
                }
                if (blockScript.permLeft)
                {
                    blockScript.permLeft = false;
                    blockScript.filledLeft = false;
                }
                if (blockScript.permTop)
                {
                    blockScript.permTop = false;
                    blockScript.filledTop = false;
                }
                if (blockScript.permRight)
                {
                    blockScript.permRight = false;
                    blockScript.filledRight = false;
                }
                
            }
        }
    }

    private Block fetchBlockScriptByIndex(int TriangleIndex)
    {
        int rowIndex = Mathf.FloorToInt(TriangleIndex / (width * 4));
        int columnIndex = Mathf.FloorToInt((TriangleIndex % (width * 4.0f) / 4.0f));
        try
        {
            Block blockScript = this.transform.GetChild(rowIndex).GetChild(columnIndex).GetComponent<Block>();
            return blockScript;
        }
        catch
        {
            Debug.Log("Triangle Index: " + TriangleIndex);
            Debug.Log("Row Index: " + rowIndex);
            Debug.Log("Column Index: " + columnIndex);
            return null;
        }
    }

    public void setPerm(List<int> triangleIndices)
    {
        for (int i = 0; i < triangleIndices.Count; ++i)
        {
            Block currentBlockScript = fetchBlockScriptByIndex(triangleIndices[i]);
            int quadrantIndex = (triangleIndices[i] % (width * 4) % 4);
            if(quadrantIndex == 0)
            {
                currentBlockScript.permBottom = true;
            }
            if (quadrantIndex == 1)
            {
                currentBlockScript.permLeft = true;
            }
            if (quadrantIndex == 2)
            {
                currentBlockScript.permTop = true;
            }
            if (quadrantIndex == 3)
            {
                currentBlockScript.permRight = true;
            }
        }
        swapLock = false;
    }

    public void clearFilledRows()
    {
        bool fullRowFound = false;
        //Debug.Log("Starting: " + fullRowFound.ToString());
        int counter = 0;
        for(int i  = 0; i < height; ++i)
        {
            //Debug.Log("i/" + i);
            for(int j = 0; j < width; ++j)
            {
                //Debug.Log("j/" + j);
                //Debug.Log(i * width * 4 + j * 4);
                Block blockScript =  fetchBlockScriptByIndex(i * width * 4 + j * 4);
                //Debug.Log(blockScript.permBottom.ToString() + " " + blockScript.permLeft.ToString() + " " + blockScript.permTop.ToString() + " " + blockScript.permRight.ToString() + " ");
                if (!blockScript.permBottom || !blockScript.permLeft || !blockScript.permTop || !blockScript.permRight)
                {
                    if (fullRowFound)
                    {
                        if(i > 1)
                        {
                            Debug.Log("Clearing beneath");
                            Debug.Log("i - 1: " + (i - 2).ToString());
                            for (int l = 0; l < width; ++l)
                            {
                                Block beneathBlockScript = fetchBlockScriptByIndex((i- 2) * width * 4 + l * 4);
                                if(beneathBlockScript.permTop && !beneathBlockScript.permBottom)
                                {
                                    Debug.Log("l: " + l.ToString());
                                    beneathBlockScript.filledBottom = false;
                                    beneathBlockScript.filledLeft = false;
                                    beneathBlockScript.filledTop = false;
                                    beneathBlockScript.filledRight = false;
                                    beneathBlockScript.permBottom = false;
                                    beneathBlockScript.permLeft = false;
                                    beneathBlockScript.permTop = false;
                                    beneathBlockScript.permRight = false;
                                }
                            }
                        }
                        for(int k = i; k < height; ++k)
                        {
                            for(int l = 0; l < width; ++l)
                            {
                                Block upperBlockScript = fetchBlockScriptByIndex(k * width * 4 + l * 4);
                                Block lowerBlockScript = fetchBlockScriptByIndex((k - counter) * width * 4 + l * 4);
                                lowerBlockScript.permBottom = upperBlockScript.permBottom;
                                lowerBlockScript.permLeft = upperBlockScript.permLeft;
                                lowerBlockScript.permTop = upperBlockScript.permTop;
                                lowerBlockScript.permRight = upperBlockScript.permRight;
                                lowerBlockScript.filledBottom = upperBlockScript.permBottom;
                                lowerBlockScript.filledLeft = upperBlockScript.permLeft;
                                lowerBlockScript.filledTop = upperBlockScript.permTop;
                                lowerBlockScript.filledRight = upperBlockScript.permRight;

                                Transform upperBlockTriangles = upperBlockScript.gameObject.transform.Find("triangles");
                                Transform lowerBlockTriangles = lowerBlockScript.gameObject.transform.Find("triangles");

                                for(int m = 0; m < 4; ++m)
                                {
                                    lowerBlockTriangles.GetChild(m).GetComponent<SpriteRenderer>().color = upperBlockTriangles.GetChild(m).GetComponent<SpriteRenderer>().color;
                                }
                            }
                        }
                        rowsCleared += counter;
                        scoreText.text = "Rows Cleared: " + rowsCleared.ToString();
                        counter = 0;
                    }
                    fullRowFound = false;
                    break;
                }
                else if(j == width - 1)
                {
                    fullRowFound = true;
                    ++counter;
                }
            }
            //Debug.Log(i.ToString() + ": " + fullRowFound);
        }
    }

    public int getHeight()
    {
        return height;
    }

    public int getWidth()
    {
        return width;
    }


    public void dropGhostPiece()
    {
        if(ghostOn)
        {
            emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
            ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
            ghostPiece.GetComponent<Piece>().ghostDrop();
        }
    }
    
    // Use this for initialization
    void Start () {
        generateBoard();
        //generatePiece();
        ghostPiece = Instantiate(ghostPiece);
        ghostPiece.GetComponent<Piece>().gameBoard = this;

    }
	
	// Update is called once per frame
	void Update () {
        if(currentPiece == null && pieceQueue != null && !gameOver)
        {
            GameObject piecePrefab = pieceQueue.GetComponent<PieceQueue>().Dequeue();
            currentPiece = Instantiate(piecePrefab);
            currentPiece.GetComponent<Piece>().gameBoard = this;
            ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>();
            ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
            //ghostPiece.GetComponent<Piece>().ghostDrop();
            //dropGhostPiece();

        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            obilterateBoard();
        }
        if (Input.GetKeyDown(KeyCode.R) && gameOver)
        {
            resetGame();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && pieceHolder != null && !swapLock && !gameOver)
        {
            emptyTriangles(currentPiece.GetComponent<Piece>().getTriangleIndices());
            emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
            currentPiece.GetComponent<Piece>().activate(false);
            GameObject heldPiece = pieceHolder.GetComponent<pieceHolder>().swap(currentPiece);
            currentPiece = null;
            if(heldPiece != null)
            {
                currentPiece = Instantiate(heldPiece);
                currentPiece.GetComponent<Piece>().resetPosition();
                Destroy(heldPiece);
            }
            swapLock = true;
            //ghostPiece.GetComponent<Piece>().ghostDrop();
            //dropGhostPiece();
        }

        if(Input.GetKeyDown(KeyCode.G) && !gameOver)
        {
            ghostOn = !ghostOn;
            if (ghostOn)
            {
                dropGhostPiece();
            }
            else
            {
                emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
            }
        }
	}


}
