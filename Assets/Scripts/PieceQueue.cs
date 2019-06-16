using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceQueue : MonoBehaviour {

    public int pieceListIndex;
    public List<GameObject> pieces;
    public List<GameObject> pieces1;
    //public List<GameObject> sprites;
    public List<GameObject> pieceQueue;
    //public List<GameObject> spriteQueue;

    void fillQueue()
    {
        List<GameObject> pieceBucket = null;
        if (pieceListIndex == 0)
        {
            pieceBucket = new List<GameObject>(pieces);
        }
        else if(pieceListIndex == 1)
        {
            pieceBucket = new List<GameObject>(pieces1);
        }
        //List<GameObject> spriteBucket = new List<GameObject>(sprites);
        while (pieceBucket.Count > 0)
        {
            int pieceIndex = Random.Range(0, pieceBucket.Count);
            pieceQueue.Add(pieceBucket[pieceIndex]);
            //spriteQueue.Add(spriteBucket[pieceIndex]);
            pieceBucket.RemoveAt(pieceIndex);
            //spriteBucket.RemoveAt(pieceIndex);
        }
        updateUI();

    }

    void updateUI()
    {
       for(int i = 1; i < this.transform.childCount; ++i)
       {
            Destroy(this.transform.GetChild(i).gameObject);
       } 
        for(int i = 0; i < 5; ++i)
        {
            GameObject sprite = Instantiate(pieceQueue[i].GetComponent<Piece>().getSprite(), this.transform.position - new Vector3(0, 1.5f * i + (i > 0 ? 1.0f : 0)), Quaternion.Euler(0, 0, 0));
            sprite.transform.parent = this.transform;
            if(i == 0)
            {
                sprite.GetComponent<Transform>().localScale = new Vector3(0.75f, 0.75f, 1.0f);
            }
        }
    }

    public GameObject Dequeue()
    {
        GameObject frontPiece = pieceQueue[0];
        pieceQueue.RemoveAt(0);
        //spriteQueue.RemoveAt(0);
        if (pieceQueue.Count < 5)
        {
            fillQueue();
        }
        updateUI();
        return frontPiece;

    }

	// Use this for initialization
	void Start () {
        fillQueue();
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(pieceQueue.Count);
        //for(int i = 0; i < queue.Count; ++i)
        //{
        //    Debug.Log(queue[i].name);
        //}

    }
}
