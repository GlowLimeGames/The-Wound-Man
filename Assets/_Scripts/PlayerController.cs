using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public List<ItemController> inventory;
    public List<ItemController> usedInventory;

    private GameController _gm;
	//private float _moveSpeed = 3.0f;
	//private Vector3 _moveTarget;

	//// Was the keyboard the last method used for movement input?
	//private bool _keyboardLast;

	// Use this for initialization
	void Start () {
        // When using a list instead of an array, you put it as the first argument intead of doing an assignment statement.
        GetComponentsInChildren<ItemController>(inventory);

        _gm = GameObject.Find("GameController").GetComponent<GameController>();
        //_moveTarget = transform.position;
		//_keyboardLast = true;
	}
	
	// Update is called once per frame
	void Update () {
        // TODO: Movement disabled for now, looking like we won't be moving this way
        /*
		if (Input.GetAxis ("Horizontal") != 0.0f) {
			_keyboardLast = true;
			var x = Input.GetAxis ("Horizontal") * Time.deltaTime * _moveSpeed;
			transform.Translate (x, 0, 0);

		} else {

            if (_gm.itemOnMouse == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _keyboardLast = false;
                    Camera cam = Camera.main;
                    _moveTarget = cam.ScreenToWorldPoint(Input.mousePosition);
                    _moveTarget.y = transform.position.y;   // Don't move vertically at all
                    _moveTarget.z = transform.position.z;

                }

                float _targetDelta = _moveTarget.x - transform.position.x;

                // Only move toward the mouse target if the keyboard's not being used now
                if (!_keyboardLast)
                {
                    if (Mathf.Abs(_targetDelta) > 0.5f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = transform.position;
                    }
                }

            }

		}
        */
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
