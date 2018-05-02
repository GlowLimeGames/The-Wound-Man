using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{
    //public float endurance;
    public GameObject objectGuarded;

    public Item.Quality requiredQuality;

    // Prefabs
    public Transform enduranceSlider;
    public Transform tooltip;

    public Canvas canv;

    // References to the actual transform
    private Transform _activeSlider;
    private Transform _activeTooltip;

	//private int _numClicks = 0;
	private int _requiredClicks;

	// Tooltip is above, or below if the obejct is high on the screen (local.y >= 3.0f)
    private Vector3 _tooltipOffset = new Vector3(0, 75, 0);

    // Slider is below
    private static Vector3 _enduranceSliderOffset = new Vector3(0, -50, 0);

    // Use this for initialization
    void Start()
    {
        // Disable clicking on the object underneath
        objectGuarded.GetComponent<BoxCollider2D>().enabled = false;

		// Objects high up in the room should display a tooltip below them
		if (transform.localPosition.y > 3.0f)
		{
			_tooltipOffset *= -1;
		}
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
	// Check if player is clicking on this obstacle with the right item
		if (GameController.Instance.itemOnMouse != null)
		{
			if (GameController.Instance.itemOnMouse.quality == requiredQuality)
			{
				if (_activeSlider == null)
				{
					_activeSlider = _initializeSlider();

					_requiredClicks = 11 - (int)GameController.Instance.itemOnMouse.efficiency;
					_activeSlider.GetComponent<Slider> ().maxValue = _requiredClicks;
					_activeSlider.GetComponent<Slider> ().value = _requiredClicks;
				}

				Slider slider = _activeSlider.GetComponent<Slider> ();

				// Decrease the obstacle's efficiency
				if (!_activeSlider.gameObject.activeSelf)
				{
					_activeSlider.gameObject.SetActive(true);
				}

				slider.value--;
				//_numClicks++;

				if (slider.value <= 0.0f)
				{
					_activeSlider.gameObject.SetActive(false);

					if (objectGuarded != null) {
						objectGuarded.GetComponent<BoxCollider2D> ().enabled = true;
					}
					Destroy (this.gameObject);
				}
					
			}
		}
    }

    void OnMouseEnter()
    {
        if (_activeTooltip == null)
        {
            _activeTooltip = _initializeTooltip();

            // Set tooltip text
            Text tooltipTextField = _activeTooltip.GetComponentInChildren<Text>();
            tooltipTextField.text = _tooltipText();
        }
    }

    void OnMouseExit()
    {
        Destroy(_activeTooltip.gameObject);
    }

    void OnDestroy()
    {
        if (_activeSlider != null)
        {
            Destroy(_activeSlider.gameObject);
        }

        if (_activeTooltip != null)
        {
            Destroy(_activeTooltip.gameObject);
        }
    }

    private string _tooltipText()
    {
        return "<b>" +  this.name + "</b>\nRequired: " + requiredQuality;
    }

    private Transform _initializeTooltip()
    {
        _activeTooltip = Instantiate(tooltip, Vector3.zero, Quaternion.identity);
		_activeTooltip.SetParent(canv.transform, false);

        // Set activeTooltip to be above the object
        Vector3 p = Camera.main.WorldToScreenPoint(transform.position);
        p += _tooltipOffset;
        _activeTooltip.transform.position = p;

        return _activeTooltip;
    }

    private Transform _initializeSlider()
    {
        _activeSlider = Instantiate(enduranceSlider, Vector3.zero, Quaternion.identity);
        // Set enduranceSlider to be above the object
        _activeSlider.SetParent(canv.transform, false);

        Vector3 p = Camera.main.WorldToScreenPoint(transform.position);
        p += _enduranceSliderOffset;

        _activeSlider.position = p;
        return _activeSlider;
    }
}