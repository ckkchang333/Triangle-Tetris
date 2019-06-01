using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceQueue : MonoBehaviour {

    public List<GameObject> pieces;
    public List<GameObject> sprites;
    public List<GameObject> pieceQueue;
    public List<GameObject> spriteQueue;

    void fillQueue()
    {
        List<GameObject> pieceBucket = pieces;
        List<GameObject> spriteBucket = sprites;
        while (pieceBucket.Count > 0)
        {
            int pieceIndex = Random.Range(0, pieceBucket.Count);
            pieceQueue.Add(pieceBucket[pieceIndex]);
            spriteQueue.Add(sprites[pieceIndex]);
            pieceBucket.RemoveAt(pieceIndex);
            spriteBucket.RemoveAt(pieceIndex);
        }
        updateUI();

    }

    void updateUI()
    {
       for(int i = 0; i < this.transform.childCount; ++i)
       {
            Destroy(this.transform.GetChild(i));
       } 
        for(int i = 0; i < spriteQueue.Count; ++i)
        {
            GameObject sprite = Instantiate(spriteQueue[i], this.transform.position - new Vector3(0, 1.5f * i), Quaternion.Euler(0, 0, 0));
            sprite.transform.parent = this.transform;
        }
    }

	// Use this for initialization
	void Start () {
        fillQueue();
    }
	
	// Update is called once per frame
	void Update () {
		//if(queue.Count < 5)
  //      {
  //          fillQueue();
  //      }
        Debug.Log(pieceQueue.Count);
        //for(int i = 0; i < queue.Count; ++i)
        //{
        //    Debug.Log(queue[i].name);
        //}

    }
}
