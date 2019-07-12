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
    public Text startText;
    public Text pausedText;
    public List<GameObject> titlePieces;
    public GameObject uiObject;
    private UI uiController;


    public int rowsCleared = 0;

    public bool swapLock = false;

    public bool gameOver = false;
    private float pieceCurrentLowerTimer = 1.0f;

    private bool active = false;
    private bool titleActive = true;

    private bool paused = false;
    private int titleLetterIndex = 4;
    private int currentHighScore = 0;
    public int leftDelayedAutoshift;
    public int rightDelayedAutoshift;
    public int leftAutoRepeatRate;
    public int rightAutoRepeatRate;


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
        currentPiece.GetComponent<Piece>().toggleActive(false);
        bool newHighScore = rowsCleared > currentHighScore;
        if (newHighScore)
        {
            currentHighScore = rowsCleared;
        }
        uiController.endGameUI(newHighScore, currentHighScore);
        pieceQueue.SetActive(!toggle);
        pieceHolder.GetComponent<pieceHolder>().empty();
        pieceHolder.SetActive(!toggle);
        gameOver = toggle;
    }

    public void setDasAndArr(int leftDASvalue, int leftARRvalue, int rightDASvalue, int rightARRvalue)
    {
        leftDelayedAutoshift = leftDASvalue;
        leftAutoRepeatRate = leftARRvalue;
        rightDelayedAutoshift = rightDASvalue;
        rightAutoRepeatRate = rightARRvalue;
    }

    public void resetGame()
    {
        rowsCleared = 0;
        scoreText.text = "Rows Cleared: " + rowsCleared;
        obilterateBoard();
        setGameOver(false);
        pieceHolder.GetComponent<pieceHolder>().empty();
        pieceCurrentLowerTimer = 1.0f;
        if (currentPiece != null)
        {
            currentPiece.GetComponent<Piece>().active = false;
            emptyTriangles(currentPiece.GetComponent<Piece>().trianglesIndices);
            emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
            Destroy(currentPiece);

        }
        return;
    }

    public bool checkEmpty(List<int> triangleIndices)
    {
        //Debug.Log("In Check Empty Start");
        //for (int i = 0; i < triangleIndices.Count; ++i)
        //{
        //    Debug.Log(triangleIndices[i]);
        //}
        //Debug.Log("In Check Empty End");
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
                if(getColumnIndex(triangleIndices[i]) > 3 * (getWidth()/4))
                {
                    return false;
                }
            }
            else if (getColumnIndex(triangleIndices[0]) > 3 * (getWidth() / 4))
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
            //Debug.Log("Triangle Index: " + TriangleIndex);
            //Debug.Log("Row Index: " + rowIndex);
            //Debug.Log("Column Index: " + columnIndex);
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



    void pause()
    {
        paused = !paused;
        currentPiece.GetComponent<Piece>().toggleActive(!paused);
        //pausedText.gameObject.SetActive(paused);

    }

    public void clearFilledRows()
    {
        bool fullRowFound = false;
        //Debug.Log("Starting: " + fullRowFound.ToString());
        int counter = 0;
        for(int i  = 0; i < height; ++i)
        {
            for(int j = 0; j < width; ++j)
            {
                Block blockScript =  fetchBlockScriptByIndex(i * width * 4 + j * 4);
                if (!blockScript.permBottom || !blockScript.permLeft || !blockScript.permTop || !blockScript.permRight)
                {
                    if (fullRowFound)
                    {
                        if(i > 1)
                        {
                            for (int l = 0; l < width; ++l)
                            {
                                Block beneathBlockScript = fetchBlockScriptByIndex((i- 2) * width * 4 + l * 4);
                                if(beneathBlockScript.permTop && !beneathBlockScript.permBottom)
                                {
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
                                //Debug.Log("There");
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
                        //pieceCurrentLowerTimer = 1.0f - 0.1f * rowsCleared;
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
        if (ghostOn)
        {
            emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
        }
        ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().getTriangleIndices());
        ghostPiece.GetComponent<Piece>().coreTriangle = currentPiece.GetComponent<Piece>().getCoreTriangle();
        ghostPiece.GetComponent<Piece>().ghostDrop();
        if (!ghostOn)
        {
            emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
        }
    }

    public float getPieceLowerTimer()
    {
        return pieceCurrentLowerTimer;
    }

    public void toggleActive(bool toggle)
    {
        active = toggle;
    }

    public void startGame()
    {
        Destroy(currentPiece);
        obilterateBoard();
        toggleActive(true);
        gameOver = false;
        //startText.gameObject.SetActive(false);
        if(currentPiece != null)
        {
            currentPiece.GetComponent<Piece>().active = false;
            emptyTriangles(currentPiece.GetComponent<Piece>().trianglesIndices);
            emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
            Destroy(currentPiece);

        }
        ghostOn = true;
        obilterateBoard();
        pieceQueue.SetActive(true);
        pieceHolder.SetActive(true);
        pieceQueue.GetComponent<PieceQueue>().emptyQueue();
        pieceQueue.GetComponent<PieceQueue>().fillQueue();
        pieceHolder.GetComponent<pieceHolder>().empty();
        rowsCleared = 0;
        paused = false;
    }
    
    public int getGhostCoreTriangle()
    {
        return ghostPiece.GetComponent<Piece>().getCoreTriangle();
    }
    
    // Use this for initialization
    void Start () {
        uiController = uiObject.GetComponent<UI>();
        generateBoard();
        //generatePiece();
        ghostPiece = Instantiate(ghostPiece);
        ghostPiece.GetComponent<Piece>().gameBoard = this;

    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            obilterateBoard();
            currentPiece = null;
            startGame();
        }
        if(active)
        {

            if (currentPiece == null && pieceQueue != null && !gameOver)
            {
                GameObject piecePrefab = pieceQueue.GetComponent<PieceQueue>().dequeue();
                currentPiece = Instantiate(piecePrefab);
                currentPiece.GetComponent<Piece>().gameBoard = this;
                currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
                ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
                //ghostPiece.GetComponent<Piece>().ghostDrop();
                //dropGhostPiece();

            }

            if (Input.GetKeyDown(KeyCode.O))
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
                currentPiece.GetComponent<Piece>().toggleActive(false);
                GameObject heldPiece = pieceHolder.GetComponent<pieceHolder>().swap(currentPiece);
                currentPiece = null;
                if (heldPiece != null)
                {
                    heldPiece.GetComponent<Piece>().resetPosition();
                    currentPiece = Instantiate(heldPiece);
                    currentPiece.GetComponent<Piece>().resetPosition();
                    Destroy(heldPiece);
                }
                swapLock = true;
            }

            if (Input.GetKeyDown(KeyCode.G) && !gameOver)
            {
                ghostOn = !ghostOn;
                dropGhostPiece();
                if(!ghostOn)
                {
                    emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
                }
                //if (ghostOn)
                //{
                //    dropGhostPiece();
                //}
                //else
                //{
                //    emptyTriangles(ghostPiece.GetComponent<Piece>().trianglesIndices);
                //}
            }
            if((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Q)) && !gameOver)
            {
                pause();
            }

            if(Input.GetKeyDown(KeyCode.F10))
            {
                uiController.GetComponent<UI>().resetUI();
                startGame();
            }
        }
        else if(titleActive)
        {
            ghostOn = false;
            if (titleLetterIndex >= 0 && currentPiece == null)
            {
                currentPiece = Instantiate(titlePieces[titleLetterIndex]);
                //ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
                dropGhostPiece();
                if (titleLetterIndex == 0)
                {
                    currentPiece.GetComponent<Piece>().coreTriangle = (getHeight() - 5) * getWidth() * 4 + titleLetterIndex * 4;
                }
                else
                {
                    currentPiece.GetComponent<Piece>().coreTriangle = (getHeight() - 5) * getWidth() * 4 + (2 * titleLetterIndex - 1) * 4;
                }
                currentPiece.GetComponent<Piece>().gameBoard = this;
                --titleLetterIndex;
            }
            else if(titleLetterIndex == -1 && currentPiece == null)
            {

                uiController.toggleActive(true, 0);
                titleActive = false;
            }
        }
    }


}
