using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    public int lethality;
    public int efficiency;

    // Tooltip prefab
    public Transform tooltip;
    public Canvas canv;

    private Camera _cam;
    private GameController _gm;

	private Transform activeTooltip;
    private bool _onMouse;
    private bool _inBody;
    

    private Vector3 originalPos;
    private Vector3 originalLocalPos;

    private static Vector3 tooltipPos = new Vector3(0, 350, 0);

    public void TakeFromBody()
    {
        _onMouse = true;
        _inBody = false;
        _gm.itemOnMouse = this;

        _gm.animusBurnRate += lethality;
    }

    public void ReturnToBody()
    {
        transform.position = originalPos;
        transform.localPosition = originalLocalPos;
        _onMouse = false;
        _inBody = true;
        _gm.itemOnMouse = null;
        Destroy(activeTooltip.gameObject);

        _gm.animusBurnRate -= lethality;
    }

	// Use this for initialization
	void Start () {
        _onMouse = false;
        _inBody = true;
        print(transform.localPosition);
        print(transform.position);
        originalPos = transform.position;
        originalLocalPos = transform.localPosition;

        // TODO Don't hard-code
        lethality = 9;
        efficiency = 7;

        _gm = GameObject.Find("GameController").GetComponent<GameController>();
        _cam = Camera.main;

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

    void OnMouseDown()
    {
        if (!_onMouse)
        {
            TakeFromBody();

        } else
        {
            _onMouse = false;
            _gm.itemOnMouse = null;
        }
    }

	void OnMouseOver() {

		if (activeTooltip == null) {
			activeTooltip = Instantiate (tooltip, tooltipPos, Quaternion.identity);

			// Convert from world position to canvas position
            // (Not necessary after switching to Overlay canvas mode)
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

    void OnDestroy()
    {
        Destroy(activeTooltip.gameObject);
    }
}

// When I click on the target, check if the right object is "on the mouse"
// (But currently if I click on anything, it clicks on the on-mouse object...)
// For now I'll just make the item track above the mouse