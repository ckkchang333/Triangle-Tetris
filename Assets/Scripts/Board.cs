using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    [Header("Board Parameters")]
    public int length = 6;              // in number of grids
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

    void generateBoard ()
    {
        for (int i = 0; i < height; ++i)
        {
            GameObject gridRow = Instantiate(blockRow, new Vector3(originX + gridWidth / 2, originY + i * (gridHeight - gridSideThickness / 2), 0), Quaternion.Euler(0, 0, 0));
            gridRow.transform.parent = this.transform;
            for (int j = 0; j < length; ++j)
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
            currentBlockScript.setQuadFilled((triangleIndices[i] % (length * 4) % 4), false, Color.clear);
        }
    }

    public void updateBoard(List<int> currentTriangleIndices, Color pieceColor)
    {
        // Fill in board according to the triangles that the piece occupies
        Piece pieceScript = currentPiece.GetComponent<Piece>();
        for (int i = 0; i < pieceScript.trianglesIndices.Count; ++i)
        {
            Block currentBlockScript = fetchBlockScriptByIndex(currentTriangleIndices[i]);
            int quadIndex = (currentTriangleIndices[i] % (length * 4) % 4);
            currentBlockScript.setQuadFilled(quadIndex, true, pieceColor);
        }
    }

    public bool checkEmptyBelow(List<int> triangleIndices)
    {
        for (int i = 0; i < triangleIndices.Count; ++i)
        {
            if(triangleIndices[i] < length * 4)
            {
                return false;
            }
            int triangleIndexBelow = triangleIndices[i] - length * 4;
            Block currentBlockScript = fetchBlockScriptByIndex(triangleIndexBelow);
            int quadrantIndex = (triangleIndexBelow % (length * 4) % 4);
            if ((quadrantIndex == 0 && currentBlockScript.permBottom) &&
                (quadrantIndex == 1 && currentBlockScript.permLeft) && 
                (quadrantIndex == 2 && currentBlockScript.permTop) &&
                (quadrantIndex == 3 && currentBlockScript.permRight))
            {
                return false;
            }
        }
        return true;
    }

    private Block fetchBlockScriptByIndex(int TriangleIndex)
    {
        int rowIndex = TriangleIndex / (length * 4);
        int columnIndex = Mathf.FloorToInt((TriangleIndex % (length * 4.0f) / 4.0f));
        Block blockScript = this.transform.GetChild(rowIndex).GetChild(columnIndex).GetComponent<Block>();
        return blockScript;
    }

    // Use this for initialization
    void Start () {
        generateBoard();
        generatePiece();

    }
	
	// Update is called once per frame
	void Update () {
	}


}
