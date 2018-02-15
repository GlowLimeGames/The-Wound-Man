using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour {

    private GameController _gm;

	// Use this for initialization
	void Start () {
        _gm = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        // TODO Don't hard-code this
        if (_gm.itemOnMouse.CompareTag("Sword"))
        {
            // TODO: Do things with animus
            Destroy(_gm.itemOnMouse.gameObject);
            
            Destroy(this.gameObject);

            _gm.itemOnMouse = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Player items should have a boxcollider2D and a rigidbody2D set to d ynamic, with a gravity scale 0.
        // That's one way to get the triggers to happen
        print(other);
    }
}
