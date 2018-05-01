using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{

    public float endurance;
    public GameObject objectGuarded;

    public Item.Quality requiredQuality;

    // Prefabs
    public Transform enduranceSlider;
    public Transform tooltip;

    public Canvas canv;

    // References to the actual transform
    private Transform _activeSlider;
    private Transform _activeTooltip;

    private float clickCounter;
    private float check = 0.0f;

    // Tooltip is above (unless that's too high on the screen)
    private Vector3 _tooltipOffset = new Vector3(0, 75, 0);

    // Slider is below
    private static Vector3 _enduranceSliderOffset = new Vector3(0, -50, 0);

    // Use this for initialization
    void Start()
    {
        // Disable clicking on the object underneath
        objectGuarded.GetComponent<BoxCollider2D>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (endurance <= 0.0)
        {
            // Enable clicking on the guarded object
            if (objectGuarded != null)
            {
                objectGuarded.GetComponent<BoxCollider2D>().enabled = true;
            }

            GameController.Instance.itemOnMouse.DoneUsing();
            Destroy(this.gameObject);
        }
    }

    void OnMouseOver()
    {
        // Check if player is clicking on this obstacle with the right item
        if (Input.GetMouseButton(0))
        {
            if (GameController.Instance.itemOnMouse != null)
            {

                if (GameController.Instance.itemOnMouse.quality == requiredQuality)
                {
                    if (_activeSlider == null)
                    {
                        _activeSlider = _initializeSlider();
                    }

                    // Decrease the obstacle's efficiency
                    if (!_activeSlider.gameObject.activeSelf)
                    {
                        clickCounter = GameController.Instance.itemOnMouse.efficiency;
                        _activeSlider.gameObject.SetActive(true);
                        GameController.Instance.itemOnMouse.Use();
                    }

                    endurance -= GameController.Instance.itemOnMouse.efficiency * Time.deltaTime;

                    if (endurance <= 0.0f)
                    {
                        _activeSlider.gameObject.SetActive(false);
                        print("slider destroyed");
                    }
                    //                    decrementAmount = endurance/GameController.Instance.itemOnMouse.efficiency;
                    //                    endurance = endurance - decrementAmount;

                    _activeSlider.GetComponent<Slider>().value = endurance;
                    //                    print (enduranceSlider.value);
                }
            }
        }
        else
        {
            // If mouse isn't down, pause the animus
            if (GameController.Instance.itemOnMouse != null)
            {
                GameController.Instance.itemOnMouse.DoneUsing();
            }
        }
    }

    void OnMouseDown()
    {

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
        return this.name + "\nRequired: " + requiredQuality;
    }

    private Transform _initializeTooltip()
    {
        _activeTooltip = Instantiate(tooltip, Vector3.zero, Quaternion.identity);

        // Set activeTooltip to be above the object
        Vector3 p = Camera.main.WorldToScreenPoint(transform.position);

        // Objects high up in the room should display a tooltip below them
        if ((transform.localPosition.y > 3.0f) && (_tooltipOffset.y > 0.0f))
        {
            _tooltipOffset *= -1;
        }
        p += _tooltipOffset;

        _activeTooltip.SetParent(canv.transform, false);

        _activeTooltip.transform.position = p;

        return _activeTooltip;
    }

    private Transform _initializeSlider()
    {
        Transform activeSlider = Instantiate(enduranceSlider, Vector3.zero, Quaternion.identity);
        // Set enduranceSlider to be above the object
        activeSlider.SetParent(canv.transform, false);

        Vector3 p = Camera.main.WorldToScreenPoint(transform.position);
        p += _enduranceSliderOffset;

        activeSlider.position = p;
        return activeSlider;
    }
}