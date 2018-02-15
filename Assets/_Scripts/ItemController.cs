using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour {

    public float lethality;
    public float efficiency;

    public enum Quality
    {
        Sharp,
        Heavy,
        Narrow,
    };
    public Quality quality;

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

    private static Vector3 tooltipPos = new Vector3(0, 300, 0);

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

        lethality = Random.value * 10.0f;
        efficiency = Random.value * 10.0f;

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

            // Set tooltip text
            Text tooltipTextField = activeTooltip.GetComponentInChildren<Text>();
            tooltipTextField.text = _tooltipText();

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

    private string _tooltipText()
    {
        return this.name + "\n" + quality + "\n" + "Lethality: " + Mathf.Round(lethality).ToString() + "\n" + "Efficiency: " + Mathf.Round(efficiency).ToString();
    }
}

// When I click on the target, check if the right object is "on the mouse"
// (But currently if I click on anything, it clicks on the on-mouse object...)
// For now I'll just make the item track above the mouse