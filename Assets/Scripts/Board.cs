using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    [Header("Board Parameters")]
    public int width = 6;              // in number of grids
    public int height = 12;             // in number of grids
    public float originX = -3.0f;
    public float originY = -4.0f;

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


    [Header("External Components")]
    public GameObject pieceQueue;
    public GameObject pieceHolder;

    public bool swapLock = false;

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
        }
        return true;
    }

    private Block fetchBlockScriptByIndex(int TriangleIndex)
    {
        int rowIndex = TriangleIndex / (width * 4);
        int columnIndex = Mathf.FloorToInt((TriangleIndex % (width * 4.0f) / 4.0f));
        Block blockScript = this.transform.GetChild(rowIndex).GetChild(columnIndex).GetComponent<Block>();
        return blockScript;
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
                        //Debug.Log("Clearing Row");
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

    // Use this for initialization
    void Start () {
        generateBoard();
        //generatePiece();

    }
	
	// Update is called once per frame
	void Update () {
        if(currentPiece == null && pieceQueue != null)
        {
            GameObject piecePrefab = pieceQueue.GetComponent<PieceQueue>().Dequeue();
            currentPiece = Instantiate(piecePrefab);
            currentPiece.GetComponent<Piece>().gameBoard = this;
            
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && pieceHolder != null && !swapLock)
        {
            emptyTriangles(currentPiece.GetComponent<Piece>().getTriangleIndices());
            currentPiece.GetComponent<Piece>().activate(false);
            GameObject heldPiece = pieceHolder.GetComponent<pieceHolder>().swap(currentPiece);
            currentPiece = null;
            if(heldPiece != null)
            {
                currentPiece = Instantiate(heldPiece);
                Destroy(heldPiece);
            }
            swapLock = true;
        }
	}


}
