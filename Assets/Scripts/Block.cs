using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    [Header("Block Parameters")]
    //public float originX = 0.0f;        // x position of the bottomleft-most corner of the block object
    //public float originY = 0.0f;        // y position of the bottomleft-most corner of the block object

    public float width = 1.0f;         // horizontal distance of the block
    public float height = 1.0f;         // vertical distance of the block

    public float sideThickness = 0.05f;     // block's sides thickness


    public float crossingThickness = 0.05f;     // block's sides thickness

    public bool octal;

    public bool filledBottom, filledLeft, filledTop, filledRight;           // Activates the triangles to form the shapes
    public bool permBottom, permLeft, permTop, permRight;           // Activates the triangles to form the shapes

    public float triangleScaleModifierY = 1.0f;
    

    public void setSize(float newWidth, float newHeight, float newSideThickness, float newCrossingThickness)
    {
        width = newWidth;
        height = newHeight;
        sideThickness = newSideThickness;
        crossingThickness = newCrossingThickness;
    }

    public void emptyGrid()
    {
        filledBottom = false || permBottom;
        filledLeft = false || permLeft;
        filledTop = false || permTop;
        filledRight = false || permRight;
    }

    public void setQuadFilled(int index, bool fill, Color newColor)
    {
        if (index == 0)
        {
            filledBottom = fill;
            this.transform.Find("triangles").Find("triangleBottom").GetComponent<SpriteRenderer>().color = newColor;
        }
        if (index == 1)
        {
            filledLeft = fill;
            this.transform.Find("triangles").Find("triangleLeft").GetComponent<SpriteRenderer>().color = newColor;
        }
        if (index == 2)
        {
            filledTop = fill;
            this.transform.Find("triangles").Find("triangleTop").GetComponent<SpriteRenderer>().color = newColor;
        }
        if (index == 3)
        {
            filledRight = fill;
            this.transform.Find("triangles").Find("triangleRight").GetComponent<SpriteRenderer>().color = newColor;
        }
    }


	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update () {

        // Ensure valid thickness
        if(sideThickness > width / 2.0f - width / 3.0f)
        {
            sideThickness = width / 2.0f - width / 3.0f;
        }
        if (sideThickness > height / 2.0f - height / 3.0f)
        {
            sideThickness = height / 2.0f - height / 3.0f;
        }
        if (sideThickness < 0.0f)
        {
            sideThickness = 0.01f;
        }





        // Sides Scale and Position Updates

        // Set the scale and position of Bottom side 
        Transform bottomTransform = this.transform.Find("lines").Find("blockSideBottom");
        bottomTransform.localScale = new Vector3(width, sideThickness);
        bottomTransform.localPosition = new Vector3(0, -height / 2 + sideThickness / 2);


        // Set the scale and position of Left side 
        Transform leftTransform = this.transform.Find("lines").Find("blockSideLeft");
        leftTransform.localScale = new Vector3(sideThickness, height);
        leftTransform.localPosition = new Vector3(-width / 2 + sideThickness / 2, 0);
        
        // Set the scale and position of Top side 
        Transform topTransform = this.transform.Find("lines").Find("blockSideTop");
        topTransform.localScale = new Vector3(width, sideThickness);
        topTransform.localPosition = new Vector3(0, height / 2 - sideThickness / 2);

        // Set the scale and position of Right side 
        Transform rightTransform = this.transform.Find("lines").Find("blockSideRight");
        rightTransform.localScale = new Vector3(sideThickness, height);
        rightTransform.localPosition = new Vector3(width / 2 - sideThickness / 2, 0);






        // Diagonal Scale, Roation Updates
        float sideCenterWidth = width - crossingThickness;
        float sideCenterHeight = height - crossingThickness;
        float diagonalAbsoluteAngle = Mathf.Rad2Deg * Mathf.Atan(sideCenterHeight / sideCenterWidth);
        //float diagonalDistance = Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)) * diagonalScaling;
        float diagonalDistance = Mathf.Sqrt(Mathf.Pow(sideCenterWidth, 2) + Mathf.Pow(sideCenterHeight, 2));

        // Set the scale of the top left diagonal line
        Transform topLeftDiagonalTransform = this.transform.Find("lines").Find("blockTopLeftDiagonal");
        topLeftDiagonalTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, -diagonalAbsoluteAngle);
        topLeftDiagonalTransform.localScale = new Vector3(diagonalDistance, crossingThickness);

        // Set the scale of the top right diagonal line
        Transform topRightDiagonalTransform = this.transform.Find("lines").Find("blockTopRightDiagonal");
        topRightDiagonalTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, diagonalAbsoluteAngle);
        topRightDiagonalTransform.localScale = new Vector3(diagonalDistance, crossingThickness);


        // Set thickness of center horizontal and vertical lines
        if(octal)
        {
            this.transform.Find("lines").Find("blockCenterHori").localScale = new Vector3(1.0f, crossingThickness);

            this.transform.Find("lines").Find("blockCenterVerti").localScale = new Vector3(crossingThickness, 1.0f);
        }


      


        // Triangle Scale and Position Updates

        float triangleScaleRatio = 0.57681f;
        float trianglePositionRatio = 0.305908f;

        //Set bottom triangle scale and position
        Transform triangleBottom = this.transform.Find("triangles").Find("triangleBottom");
        triangleBottom.localScale = new Vector3(width, triangleScaleRatio * (height + sideThickness) * triangleScaleModifierY);
        triangleBottom.localPosition = new Vector3(0, -(trianglePositionRatio * height));

        // Set left triangle scale and position
        Transform triangleLeft = this.transform.Find("triangles").Find("triangleLeft");
        triangleLeft.localScale = new Vector3(height, triangleScaleRatio * (width + sideThickness) * triangleScaleModifierY);
        triangleLeft.localPosition = new Vector3(-(trianglePositionRatio * width), 0);

        // Set top triangle scale and position
        Transform triangleTop = this.transform.Find("triangles").Find("triangleTop");
        triangleTop.localScale = new Vector3(width, triangleScaleRatio * (height + sideThickness) * triangleScaleModifierY);
        triangleTop.localPosition = new Vector3(0, (trianglePositionRatio * height));

        // Set right triangle scale and position
        Transform triangleRight = this.transform.Find("triangles").Find("triangleRight");
        triangleRight.localScale = new Vector3(height, triangleScaleRatio * (width + sideThickness) * triangleScaleModifierY);
        triangleRight.localPosition = new Vector3((trianglePositionRatio * width), 0);



        // Activate/Deactivate triangle objects if corresponding bool is true
        triangleBottom.gameObject.SetActive(filledBottom);
        triangleLeft.gameObject.SetActive(filledLeft);
        triangleTop.gameObject.SetActive(filledTop);
        triangleRight.gameObject.SetActive(filledRight);
    }





}
