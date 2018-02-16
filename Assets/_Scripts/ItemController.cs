﻿using System.Collections;
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

    // TODO: This might change to a List<Quality>
    public Quality quality;

    // Tooltip prefab
    public Transform tooltip;
    public Canvas canv;

    private Camera _cam;
    private GameController _gm;
    private PlayerController _pc;

	private Transform _activeTooltip;
    private bool _onMouse;

    private Vector3 _originalPos;
    private Vector3 _originalLocalPos;

    private static Vector3 _tooltipPos = new Vector3(0, 250, 0);

    public void TakeFromBody()
    {
        _onMouse = true;

        _gm.itemOnMouse = this;

        _gm.animusBurnRate += lethality;

        // Allow this to show up in death messages
        _pc.usedInventory.Add(this);

        // Disable this collider, so you can have it on the mouse but still click things
        GetComponent<BoxCollider2D>().enabled = false;

        // Enable the player collider, so it can be clicked to return it to the body
        _pc.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ReturnToBody()
    {
        _onMouse = false;

        // Put it back where it was
        // TODO: Might be more fun just to put it where clicked, so you can rearrange your inventory
        transform.position = _originalPos;
        transform.localPosition = _originalLocalPos;

        // Enable its collider again so it can be clicked
        GetComponent<BoxCollider2D>().enabled = true;

        // Disable player collider, to allow items to be clicked instead
        _pc.GetComponent<BoxCollider2D>().enabled = false;

        _gm.itemOnMouse = null;
        Destroy(_activeTooltip.gameObject);

        _gm.animusBurnRate -= lethality;
    }

	// Use this for initialization
	void Start () {
        _onMouse = false;
        _originalPos = transform.position;
        _originalLocalPos = transform.localPosition;

        lethality = Random.value * 10.0f;
        efficiency = Random.value * 10.0f;

        _gm = GameObject.Find("GameController").GetComponent<GameController>();
        _pc = GetComponentInParent<PlayerController>();
        _cam = Camera.main;

    }
	
	// Update is called once per frame
	void Update () {
		if (_onMouse)
        {
            // Item moves with the cursor
            Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0.0f;                // Otherwise it gets set to -10 for some reason, and becomes invisible
            //mousePos.y = mousePos.y + 1.0f;   // Item moves slightly above the cursor
            transform.position = mousePos;
        }
	}

    void OnMouseDown()
    {
        if (!_onMouse)
        {
            TakeFromBody();

        } //else
        //{
        //    _onMouse = false;
        //    _gm.itemOnMouse = null;
        //}
    }

	void OnMouseEnter() {

		if (_activeTooltip == null) {
			_activeTooltip = Instantiate (tooltip, _tooltipPos, Quaternion.identity);

            // Set tooltip text
            Text tooltipTextField = _activeTooltip.GetComponentInChildren<Text>();
            tooltipTextField.text = _tooltipText();

			_activeTooltip.SetParent (canv.transform, false);
		}

	}

	void OnMouseExit() {
        if (!_onMouse)
        {
            Destroy(_activeTooltip.gameObject);
        }
    }

    void OnDestroy()
    {
        if (_activeTooltip != null)
        {
            Destroy(_activeTooltip.gameObject);
        }
    }

    private string _tooltipText()
    {
        return this.name + "\n" + quality + "\n" + "Lethality: " + Mathf.Round(lethality).ToString() + "\n" + "Efficiency: " + Mathf.Round(efficiency).ToString();
    }
}