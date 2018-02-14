using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    private GameController _gm;
    public Transform tooltip;
	public Canvas canv;
	private Transform activeTooltip;
    private bool _onMouse;
    private Camera _cam;

	// Use this for initialization
	void Start () {
        _cam = Camera.main;
        _onMouse = false;
        _gm = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
		if (_onMouse)
        {
            // Item moves with the cursor
            Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0.0f;                // Otherwise it gets set to -10 for some reason, and becomes invisible
            mousePos.y = mousePos.y + 1.0f;   // Item moves slightly above the cursor
            transform.position = mousePos;
        }
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        print("Colliding with something");
        if (coll.gameObject.tag == "SwordTarget")
        {
            print("Collided with target");
        }
    }

    void OnMouseDown()
    {
        if (!_onMouse)
        {
            _onMouse = true;
            _gm.itemOnMouse = this;

        } else
        {
            _onMouse = false;
            _gm.itemOnMouse = null;
        }
    }

	void OnMouseOver() {

		if (activeTooltip == null) {
			activeTooltip = Instantiate (tooltip, Vector3.zero, Quaternion.identity);

			// Convert from world position to canvas position
			//RectTransform CanvasRect = activeTooltip.GetComponent<RectTransform> ();
			//Vector3 pos = transform.position;
			//Vector2 viewportPoint = Camera.main.WorldToViewportPoint (pos);
			//CanvasRect.anchorMin = viewportPoint;
			//CanvasRect.anchorMax = viewportPoint;

			activeTooltip.SetParent (canv.transform, false);
		}

	}

	void OnMouseExit() {
        if (!_onMouse)
        {
            Destroy(activeTooltip.gameObject);
        }
    }
}

// When I click on the target, check if the right object is "on the mouse"
// (But currently if I click on anything, it clicks on the on-mouse object...)
// For now I'll just make the item track above the mouse