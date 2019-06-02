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
        List<GameObject> pieceBucket = new List<GameObject>(pieces);
        List<GameObject> spriteBucket = new List<GameObject>(sprites);
        while (pieceBucket.Count > 0)
        {
            int pieceIndex = Random.Range(0, pieceBucket.Count);
            pieceQueue.Add(pieceBucket[pieceIndex]);
            spriteQueue.Add(spriteBucket[pieceIndex]);
            pieceBucket.RemoveAt(pieceIndex);
            spriteBucket.RemoveAt(pieceIndex);
        }
        updateUI();

    }

    void updateUI()
    {
       for(int i = 0; i < this.transform.childCount; ++i)
       {
            Destroy(this.transform.GetChild(i).gameObject);
       } 
        for(int i = 0; i < 5; ++i)
        {
            GameObject sprite = Instantiate(spriteQueue[i], this.transform.position - new Vector3(0, 1.5f * i), Quaternion.Euler(0, 0, 0));
            sprite.transform.parent = this.transform;
        }
    }

    public GameObject Dequeue()
    {
        GameObject frontPiece = pieceQueue[0];
        pieceQueue.RemoveAt(0);
        spriteQueue.RemoveAt(0);
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
