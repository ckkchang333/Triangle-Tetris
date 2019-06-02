using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceHolder : MonoBehaviour {

    public GameObject heldPiece;
    public GameObject currentSprite;
    public List<GameObject> sprites;

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
        currentSprite = Instantiate(sprites[heldPiece.GetComponent<Piece>().getPieceID()], this.transform.position, Quaternion.Euler(0, 0, 0));
        return outgoing;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
