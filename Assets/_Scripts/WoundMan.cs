using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoundMan : MonoBehaviour {
    // WoundMan is the player avatar object.

    public List<Item> inventory;

    // Make it a singleton. Can access as WoundMan.Instance anywhere
    public static WoundMan Instance
    {
        get
        {
            return instance;
        }
    }
    private static WoundMan instance = null;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;

    }

    // Use this for initialization
    void Start () {
        // When using a list instead of an array, you put it as the first argument 
		// intead of doing an assignment statement.
        GetComponentsInChildren<Item>(inventory);

        // BoxCollider2D should be disabled by default. (This is redundant)
        this.GetComponent<BoxCollider2D>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

    // Clicking on the player while holding an item returns it to the body
    void OnMouseDown()
    {
		print ("Clicked on player");
        Item iom = GameController.Instance.itemOnMouse;
        if (iom != null)
        {
                iom.ReturnToBody();
        }
    }
}
