using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceHolder : MonoBehaviour {

    public GameObject heldPiece;
    public GameObject currentSprite;
    //public List<GameObject> sprites;

    public GameObject swap(GameObject incoming)
    {
        GameObject outgoing = heldPiece;
        if(outgoing != null)
        {
            outgoing.GetComponent<Piece>().resetPosition();
            outgoing.GetComponent<Piece>().activate(true);
        }
        heldPiece = incoming;
        Destroy(currentSprite);
        currentSprite = Instantiate(heldPiece.GetComponent<Piece>().getSprite(), this.transform.position, Quaternion.Euler(0, 0, 0));
        return outgoing;
    }

    public void empty()
    {
        if(heldPiece != null)
        {
            Destroy(heldPiece);
        }
        if(currentSprite != null)
        {
            Destroy(currentSprite);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
