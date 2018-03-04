using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour {

	public float endurance;
    public GameObject objectGuarded;

	// TODO: Currently this is just an object existing in the scene, a child of Canvas.
	// Not sure which is less annoying: this, or instantiating a prefab in Start()...
	public Slider enduranceSlider;

    public Item.Quality requiredQuality;

    public Transform tooltip;
    public Canvas canv;

    private Transform _activeTooltip;

    // In the middle of the screen, but below the item tooltip
    private static Vector3 _tooltipPos = new Vector3(0, 100, 0);

	private static Vector3 _enduranceSliderOffset = new Vector3 (0, 100, 0);

    // Use this for initialization
    void Start () {
        // Disable clicking on the object underneath
        objectGuarded.GetComponent<BoxCollider2D>().enabled = false;

		// Set enduranceSlider to be above the object
		Vector3 p = Camera.main.WorldToScreenPoint (transform.position);
		p += _enduranceSliderOffset;

		enduranceSlider.transform.position = p;

		// Initialize slider values
		enduranceSlider.maxValue = endurance;
		enduranceSlider.value = endurance;

		enduranceSlider.gameObject.SetActive (false);

    }
	
	// Update is called once per frame
	void Update () {
		if (endurance <= 0.0) {
			// Enable clicking on the guarded object
			if (objectGuarded != null) {
				objectGuarded.GetComponent<BoxCollider2D> ().enabled = true;
			}

			GameController.Instance.itemOnMouse.DoneUsing();
			Destroy (this.gameObject);
		}
	}

    void OnMouseOver()
    {
        // Check if player is clicking on this obstacle with the right item
		if (Input.GetMouseButton (0)) {
			if (GameController.Instance.itemOnMouse != null) {
				if (GameController.Instance.itemOnMouse.quality == requiredQuality) {
                    
                    // Decrease the obstacle's efficiency
					if (!enduranceSlider.IsActive ()) {
						enduranceSlider.gameObject.SetActive (true);
						GameController.Instance.itemOnMouse.Use ();
					}

					endurance -= GameController.Instance.itemOnMouse.efficiency * Time.deltaTime;
					enduranceSlider.value = endurance;
				}
			}
		} else {
			// If mouse isn't down, pause the animus
			if (GameController.Instance.itemOnMouse != null) {
				GameController.Instance.itemOnMouse.DoneUsing ();
			}
		}
    }
		
    void OnMouseEnter()
    {

        if (_activeTooltip == null)
        {
            _activeTooltip = Instantiate(tooltip, _tooltipPos, Quaternion.identity);

            // Set tooltip text
            Text tooltipTextField = _activeTooltip.GetComponentInChildren<Text>();
            tooltipTextField.text = _tooltipText();

            _activeTooltip.SetParent(canv.transform, false);
        }

    }

    void OnMouseExit()
    {
        Destroy(_activeTooltip.gameObject);
    }

    void OnDestroy()
    {
        if (_activeTooltip != null)
        {
            Destroy(_activeTooltip.gameObject);
        }
		Destroy (enduranceSlider.gameObject);
    }

    private string _tooltipText()
    {
        return "Required: " + requiredQuality;
    }
}
