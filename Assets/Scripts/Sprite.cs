using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite : MonoBehaviour {

    public Color spriteColor;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < this.transform.childCount; ++i)
        {
            Transform rowObject = this.transform.GetChild(i);
            for(int j = 0; j < rowObject.childCount; ++j)
            {
                Transform triangles = rowObject.GetChild(j).Find("triangles");
                for(int k = 0; k < 4; ++k)
                {
                    try
                    {
                        triangles.GetChild(k).GetComponent<SpriteRenderer>().color = spriteColor;
                    }
                    catch
                    {
                        // do nothing
                    }
                }

            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
