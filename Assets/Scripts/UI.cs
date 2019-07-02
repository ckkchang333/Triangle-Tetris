using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject mainMenuSelectorSprite;
    private bool mainMenuTop = true;
    public float displace;
    
    [Header("Other")]
    public GameObject scoreText;
    public GameObject controlsPanel;

    public bool active;
    public int uiIndex = 0;


    public void setActive(bool toggle, int currentUiIndex = 0)
    {
        active = toggle;
        uiIndex = currentUiIndex;
    }

    private void rotateSelector(bool right)
    {
        if(right)
        {
            mainMenuSelectorSprite.transform.Rotate(new Vector3(0, 0, -90));
        }
        else
        {
            mainMenuSelectorSprite.transform.Rotate(new Vector3(0, 0, 90));
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(active)
        {
            if(uiIndex == 0)
            {
                mainMenu.SetActive(true);
                mainMenuSelectorSprite.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    rotateSelector(true);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    rotateSelector(false);
                }
                if (Input.GetKeyDown(KeyCode.W) && !mainMenuTop)
                {
                    mainMenuSelectorSprite.transform.Translate(new Vector3(0, displace));
                    mainMenuTop = !mainMenuTop;

                }
                if (Input.GetKeyDown(KeyCode.S) && mainMenuTop)
                {
                    mainMenuSelectorSprite.transform.Translate(new Vector3(0, -displace));
                    mainMenuTop = !mainMenuTop;
                }
            }
        }
	}
}
