using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListWrapperGameObject
{

    public List<GameObject> myList;
}


public class PieceQueue : MonoBehaviour {

    public int pieceListIndex;
    public bool randomize;
    public List<ListWrapperGameObject> pieceLists;
    public List<GameObject> pieces;
    public List<GameObject> pieces1;
    public List<GameObject> pieceQueue;
    public List<GameObject> pieceSpriteQueue;

    public void fillQueue()
    {
        List<GameObject> pieceBucket = null;
        //if (pieceListIndex == 0)
        //{
        //    pieceBucket = new List<GameObject>(pieceLists[0].myList);
        //}
        //else if(pieceListIndex == 1)
        //{
        //    pieceBucket = new List<GameObject>(pieces1);
        //}
        pieceBucket = new List<GameObject>(pieceLists[pieceListIndex].myList);
        //List<GameObject> spriteBucket = new List<GameObject>(sprites);
        if(randomize)
        {
            while (pieceBucket.Count > 0)
            {
                int pieceIndex = Random.Range(0, pieceBucket.Count);
                pieceQueue.Add(pieceBucket[pieceIndex]);
                pieceBucket.RemoveAt(pieceIndex);
            }
        }
        else
        {
            while (pieceBucket.Count > 0)
            {
                pieceQueue.Add(pieceBucket[0]);
                pieceBucket.RemoveAt(0);
            }
        }
        updateUI();

    }

    void updateUI()
    {
       for(int i = 0; i < pieceSpriteQueue.Count; ++i)
       {
            Destroy(pieceSpriteQueue[i]);
       } 
        for(int i = 0; i < 5; ++i)
        {
            GameObject sprite = Instantiate(pieceQueue[i].GetComponent<Piece>().getSprite(), this.transform.position - new Vector3(0, 1.25f * i + (i > 0 ? 1.0f : 0)), Quaternion.Euler(0, 0, 0));
            pieceSpriteQueue.Add(sprite);
            sprite.transform.parent = this.transform;
            if(i == 0)
            {
                sprite.GetComponent<Transform>().localScale = new Vector3(0.35f, 0.35f, 1.0f);
            }
        }
    }

    public GameObject dequeue()
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

    public void emptyQueue()
    {
        pieceQueue.Clear();
    }

	// Use this for initialization
	void Start () {
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
