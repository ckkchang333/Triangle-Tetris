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
            outgoing.GetComponent<Piece>().toggleActive(true);
        }
        heldPiece = incoming;
        Destroy(currentSprite);
        currentSprite = Instantiate(heldPiece.GetComponent<Piece>().getSprite(), this.transform.localPosition, Quaternion.Euler(0, 0, 0));
        currentSprite.transform.position = new Vector3(-4.058f, 2.8f, 0);
        currentSprite.transform.parent = this.transform;
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
