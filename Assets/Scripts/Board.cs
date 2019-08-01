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
    public float pieceCurrentLowerTimer = 60;

    public bool active = false;
    private bool titleActive = true;

    private bool paused = false;
    private int titleLetterIndex = 4;
    private int currentHighScore = 0;
    public int leftDelayedAutoshift;
    public int rightDelayedAutoshift;
    public int leftAutoRepeatRate;
    public int rightAutoRepeatRate;

    public List<KeyCode> controlsKeys;
    private int holdIndex = 7;
    private int ghostIndex = 8;
    private int pauseIndex = 9;
    private int quickRestartIndex = 10;


    public int gameMode = -1;
    public int marathonRecordRows = 0;
    public float marathonRecordTime = 0;
    public int sprintRecordRows = 0;
    public float sprintRecordTime = 0;

    private int sprintTotalRows = 40;

    private bool lockDelayFlag = true;

    public float gameTime = 0;
    public List<int> marathonLevelFallIntervals;

    public void setGameMode(int newGameMode)
    {
        gameMode = newGameMode;
    }

    public void updatePieceSettings()
    {
        if(currentPiece != null)
        {
            currentPiece.GetComponent<Piece>().gameBoard = this;
            currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
            currentPiece.GetComponent<Piece>().setControlsKeys(controlsKeys);
            currentPiece.GetComponent<Piece>().setLockDelay(lockDelayFlag);
            // Playing on Rest Mode
            if(gameMode == 0)
            {
                currentPiece.GetComponent<Piece>().setSuspend(true);
            }
            //ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
        }
    }

    public void setLockDelay(bool toggle)
    {
        lockDelayFlag = toggle;
    }

    public void setControlsKeys(List<KeyCode> newKeys)
    {
        controlsKeys = new List<KeyCode>(newKeys);
    }


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
        Debug.Log("Setting Game Over");
        currentPiece.GetComponent<Piece>().toggleActive(false);
        bool newHighScoreFlag = rowsCleared > currentHighScore;
        if(gameMode == 0)
        {
            uiController.endGameUI(gameMode, false);
        }
        if (gameMode == 1)
        {
            if (rowsCleared < marathonLevelFallIntervals.Count * 10)
            {
                if (marathonRecordRows < rowsCleared)
                {
                    newHighScoreFlag = true;
                    marathonRecordRows = rowsCleared;
                    marathonRecordTime = gameTime;
                }
            }
            else
            {
                if (marathonRecordTime < gameTime)
                {
                    newHighScoreFlag = true;
                    marathonRecordRows = rowsCleared;
                    marathonRecordTime = gameTime;
                }
            }
            uiController.endGameUI(gameMode, newHighScoreFlag, marathonRecordRows, marathonRecordTime);
        }
        else if (gameMode == 2)
        {
            if (rowsCleared < 40)
            {
                if (sprintRecordRows < rowsCleared)
                {
                    newHighScoreFlag = true;
                    sprintRecordRows = rowsCleared;
                    sprintRecordTime = gameTime;
                }
            }
            else
            {
                if (sprintRecordTime < gameTime)
                {
                    newHighScoreFlag = true;
                    sprintRecordRows = rowsCleared;
                    sprintRecordTime = gameTime;
                }
            }
            uiController.endGameUI(gameMode, newHighScoreFlag, sprintRecordRows, sprintRecordTime);
        }
        //if (newHighScoreFlag)
        //{
        //    currentHighScore = rowsCleared;
        //}
        //uiController.endGameUI(gameMode, newHighScoreFlag, currentHighScore);
        pieceQueue.SetActive(!toggle);
        pieceHolder.GetComponent<pieceHolder>().empty();
        pieceHolder.SetActive(!toggle);
        gameOver = toggle;
        active = false;
    }

    public void setDasArr(int newLeftDas, int newLeftArr, int newRightDas, int newRightArr)
    {
        leftDelayedAutoshift = newLeftDas;
        leftAutoRepeatRate = newLeftArr;
        rightDelayedAutoshift = newRightDas;
        rightAutoRepeatRate = newRightArr;
        if(currentPiece != null)
        {
            currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
        }
    }

    public void resetGame()
    {
        rowsCleared = 0;
        if(gameMode == 2)
        {
            scoreText.text = "Rows Left: " + (sprintTotalRows - rowsCleared);
        }
        else
        {
            scoreText.text = "Rows Cleared: " + rowsCleared;
        }
        obilterateBoard();
        setGameOver(false);
        pieceHolder.GetComponent<pieceHolder>().empty();
        pieceCurrentLowerTimer = 60;
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

        for(int i = 0; i < triangleIndices.Count; ++i)
        {
            if(triangleIndices[i] >= (getHeight() * getWidth() * 4))
            {
                return false;
            }
        }
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
        if(currentPiece != null)
        {
            emptyTriangles(currentPiece.GetComponent<Piece>().trianglesIndices);
            Destroy(currentPiece);
        }
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
                        //if(i > 0)
                        //{
                        //    for (int l = 0; l < width; ++l)
                        //    {
                        //        Block beneathBlockScript = fetchBlockScriptByIndex((i- 1) * width * 4 + l * 4);
                        //        if(beneathBlockScript.permTop && !beneathBlockScript.permBottom)
                        //        {
                        //            Color fillColor = Color.white;
                        //            if(!beneathBlockScript.permLeft)
                        //            {
                        //                fillColor = fetchBlockScriptByIndex((i) * width * 4 + l * 4).getQuadColor(1);
                        //            }
                        //            else if(!beneathBlockScript.permRight)
                        //            {
                        //                fillColor = fetchBlockScriptByIndex((i) * width * 4 + l * 4).getQuadColor(3);
                        //            }
                        //            beneathBlockScript.setQuadFilled(0, true, fillColor);
                        //            if(!beneathBlockScript.filledLeft)
                        //            {
                        //                beneathBlockScript.setQuadFilled(1, true, fillColor);
                        //                Debug.Log("Filling Left");
                        //            }
                        //            if (!beneathBlockScript.filledRight)
                        //            {
                        //                beneathBlockScript.setQuadFilled(3, true, fillColor);
                        //                Debug.Log("Filling Right");
                        //            }
                        //            beneathBlockScript.filledBottom = true;
                        //            beneathBlockScript.filledLeft = true;
                        //            beneathBlockScript.filledTop = true;
                        //            beneathBlockScript.filledRight = true;
                        //            beneathBlockScript.permBottom = true;
                        //            beneathBlockScript.permLeft = true;
                        //            beneathBlockScript.permTop = true;
                        //            beneathBlockScript.permRight = true;
                        //        }
                        //    }
                        //}
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
                        for (int k = 0; k < counter; ++k)
                        {
                            for (int l = 0; l < width; ++l)
                            {
                                Block currentBlockScript = fetchBlockScriptByIndex((height - 1 - k) * width * 4 + l * 4);

                                currentBlockScript.permBottom = false;
                                currentBlockScript.permLeft = false;
                                currentBlockScript.permTop = false;
                                currentBlockScript.permRight = false;
                                currentBlockScript.filledBottom = false;
                                currentBlockScript.filledLeft = false;
                                currentBlockScript.filledTop = false;
                                currentBlockScript.filledRight = false;
                            }
                        }
                        rowsCleared += counter;
                        if(gameMode == 2)
                        {
                            int rowsLeft = sprintTotalRows - rowsCleared;
                            if(rowsLeft < 0)
                            {
                                rowsLeft = 0;
                                setGameOver(true);
                            }
                            scoreText.text = "Rows Left: " + rowsLeft.ToString();
                        }
                        else
                        {
                            scoreText.text = "Rows Cleared: " + rowsCleared.ToString();
                        }
                        //TODO Implement Marathon Mode
                        if(gameMode == 1)
                        {
                            int level = rowsCleared / 10;
                            if(level > 20)
                            {
                                level = 20;
                            }
                            pieceCurrentLowerTimer = marathonLevelFallIntervals[level];
                        }
                        counter = 0;
                    }
                    fullRowFound = false;
                    break;
                }
                else if(j == width - 1)
                {
                    fullRowFound = true;
                    if(i > 0)
                    {
                        for (int l = 0; l < width; ++l)
                        {
                            Block beneathBlockScript = fetchBlockScriptByIndex((i - 1) * width * 4 + l * 4);
                            if (beneathBlockScript.permTop && !beneathBlockScript.permBottom)
                            {
                                Color fillColor = Color.white;
                                if (!beneathBlockScript.permLeft)
                                {
                                    fillColor = fetchBlockScriptByIndex((i) * width * 4 + l * 4).getQuadColor(1);
                                }
                                else if (!beneathBlockScript.permRight)
                                {
                                    fillColor = fetchBlockScriptByIndex((i) * width * 4 + l * 4).getQuadColor(3);
                                }
                                beneathBlockScript.setQuadFilled(0, true, fillColor);
                                if (!beneathBlockScript.filledLeft)
                                {
                                    beneathBlockScript.setQuadFilled(1, true, fillColor);
                                    Debug.Log("Filling Left");
                                }
                                if (!beneathBlockScript.filledRight)
                                {
                                    beneathBlockScript.setQuadFilled(3, true, fillColor);
                                    Debug.Log("Filling Right");
                                }
                                beneathBlockScript.filledBottom = true;
                                beneathBlockScript.filledLeft = true;
                                beneathBlockScript.filledTop = true;
                                beneathBlockScript.filledRight = true;
                                beneathBlockScript.permBottom = true;
                                beneathBlockScript.permLeft = true;
                                beneathBlockScript.permTop = true;
                                beneathBlockScript.permRight = true;
                            }
                        }
                    }
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
        //Debug.Log("Starting new Game");
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
        gameTime = 0;
        rowsCleared = 0;
        paused = false;
        active = true;
    }
    
    public int getGhostCoreTriangle()
    {
        return ghostPiece.GetComponent<Piece>().getCoreTriangle();
    }

    public void replayTitle()
    {
        obilterateBoard();
        active = false;
        titleLetterIndex = titlePieces.Count - 1;
        titleActive = true;
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
        //if(Input.GetKeyDown(KeyCode.Return))
        //{
        //    obilterateBoard();
        //    currentPiece = null;
        //    startGame();
        //}
        if(active)
        {
            if(!paused)
            {
                gameTime += Time.deltaTime;
            }
            uiController.setGameTimer(gameTime);
            if (currentPiece == null && pieceQueue != null && !gameOver)
            {
                clearFilledRows();
                GameObject piecePrefab = pieceQueue.GetComponent<PieceQueue>().dequeue();
                currentPiece = Instantiate(piecePrefab);
                updatePieceSettings();
                //currentPiece.GetComponent<Piece>().gameBoard = this;
                //currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
                //currentPiece.GetComponent<Piece>().setControlsKeys(controlsKeys);
                //currentPiece.GetComponent<Piece>().setLockDelayFlag(lockDelayFlag);
                ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
                //ghostPiece.GetComponent<Piece>().ghostDrop();
                //dropGhostPiece();

            }

            //if (Input.GetKeyDown(KeyCode.O))
            //{
            //    obilterateBoard();
            //}
            //if (Input.GetKeyDown(KeyCode.R) && gameOver)
            //{
            //    resetGame();
            //}

            if (Input.GetKeyDown(controlsKeys[holdIndex]) && pieceHolder != null && !swapLock && !gameOver)
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
                    updatePieceSettings();
                    //currentPiece.GetComponent<Piece>().resetPosition();
                    //currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
                    //currentPiece.GetComponent<Piece>().setControlsKeys(controlsKeys);
                    Destroy(heldPiece);
                }
                swapLock = true;
            }

            if (Input.GetKeyDown(controlsKeys[ghostIndex]) && !gameOver)
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
            if(Input.GetKeyDown(controlsKeys[pauseIndex]) && !gameOver && (uiController.getUiIndex() == -2 || uiController.getUiIndex() == -1))
            {
                pause();
            }

            if(Input.GetKeyDown(controlsKeys[quickRestartIndex]))
            {
                uiController.GetComponent<UI>().resetGameUI();
                startGame();
            }
        }
        else if(titleActive)
        {
            ghostOn = false;
            if (titleLetterIndex >= 0 && currentPiece == null)
            {
                currentPiece = Instantiate(titlePieces[titleLetterIndex]);
                updatePieceSettings();
                //currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
                //currentPiece.GetComponent<Piece>().setControlsKeys(controlsKeys);
                ghostPiece.GetComponent<Piece>().trianglesIndices = new List<int>(currentPiece.GetComponent<Piece>().trianglesIndices);
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
                currentPiece.GetComponent<Piece>().setDasAndArr(leftDelayedAutoshift, leftAutoRepeatRate, rightDelayedAutoshift, rightAutoRepeatRate);
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
