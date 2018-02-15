using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour {

    public ItemController.Quality requiredQuality;

    public Transform tooltip;

    public Canvas canv;

    private GameController _gm;

    private Transform activeTooltip;

    private static Vector3 tooltipPos = new Vector3(0, 100, 0);

    // Use this for initialization
    void Start () {
        _gm = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        if (_gm.itemOnMouse.quality == requiredQuality)
        {
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

    void OnMouseOver()
    {

        if (activeTooltip == null)
        {
            activeTooltip = Instantiate(tooltip, tooltipPos, Quaternion.identity);

            // Set tooltip text
            Text tooltipTextField = activeTooltip.GetComponentInChildren<Text>();
            tooltipTextField.text = _tooltipText();

            activeTooltip.SetParent(canv.transform, false);
        }

    }

    void OnMouseExit()
    {
        Destroy(activeTooltip.gameObject);
    }

    void OnDestroy()
    {
        Destroy(activeTooltip.gameObject);
    }

    private string _tooltipText()
    {
        return "Required: " + requiredQuality;
    }
}
