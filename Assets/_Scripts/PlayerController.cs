using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public List<ItemController> inventory;

    private GameController _gm;

    // Make it a singleton. Can access as PlayerController.Instance anywhere
    public static PlayerController Instance
    {
        get
        {
            return instance;
        }
    }
    private static PlayerController instance = null;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        // When using a list instead of an array, you put it as the first argument 
		// intead of doing an assignment statement.
        GetComponentsInChildren<ItemController>(inventory);

        _gm = GameObject.Find("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        // TODO: Movement disabled for now, looking like we won't be moving this way
	}

    // Clicking on the player while holding an item returns it to the body
    void OnMouseDown()
    {
        if (_gm.itemOnMouse != null)
        {
            _gm.itemOnMouse.ReturnToBody();
        }
    }
}
