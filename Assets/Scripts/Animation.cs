using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Animation : MonoBehaviour {

    public List<ListWrapperInt> frames;
    public List<Color> buttonColorFrames;
    public int index;
    public int width;
    public int height;
    public float timer;
    public float maxTimer = 2.0f;

    //private IEnumerator play()
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(60 * Time.deltaTime);
    //        Debug.Log("Start");
    //        index = (index + 1) % 2;
    //        //clearAllTriangles();
    //        updateUI();
    //        Debug.Log("End");
    //    }
    //}

    private int getWidth()
    {
        return width;
    }

    private int getHeight()
    {
        return height;
    }

    private void updateUI()
    {
        ListWrapperInt currFrame = frames[index];
        for(int i = 0; i < currFrame.myList.Count; ++i)
        {
            Block blockScript = this.transform.GetChild(currFrame.myList[i] / (4 * getWidth())).GetChild(currFrame.myList[i] % (4 * getWidth()) / 4).GetComponent<Block>();
            blockScript.setQuadFilled(currFrame.myList[i], true, Color.clear);
        }
    }

    private void clearAllTriangles()
    {
        for(int i = 0; i < height; ++i)
        {
            Transform currRow = this.transform.GetChild(i);
            for(int j = 0; j < width; ++j)
            {
                Transform currGrid = currRow.GetChild(j);
                for(int k = 0; k < 4; ++k)
                {

                    currGrid.Find("triangles").GetChild(k).gameObject.SetActive(false);
                }
            }
        }
    }

    private void setButtonColor(Color currColor)
    {
        Transform key = this.transform.Find("D Key");
        for(int i = 0; i < 4; ++i)
        {
            key.GetChild(i).GetComponent<SpriteRenderer>().color = currColor;
        }
        key.Find("Text").GetComponent<Text>().color = currColor;
    }

    // Use this for initialization
    void Start () {
        //StartCoroutine("play");
        timer = maxTimer;
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            index = (index + 1) % 2;
            clearAllTriangles();
            updateUI();
            setButtonColor(buttonColorFrames[index]);
            timer = maxTimer;
        }
		
	}
}
