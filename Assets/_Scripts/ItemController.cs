using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

	public Transform tooltip;
	public Canvas canv;
	private Transform activeTooltip;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
		//Destroy (activeTooltip.gameObject);
	}
}
