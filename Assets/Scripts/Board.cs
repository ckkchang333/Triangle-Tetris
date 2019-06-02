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
        generatePiece();

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
