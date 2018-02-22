using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour {

    public ItemController.Quality requiredQuality;

    public Transform tooltip;
    public Canvas canv;

    private Transform _activeTooltip;

    // In the middle of the screen, but below the item tooltip
    private static Vector3 _tooltipPos = new Vector3(0, 100, 0);

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        if (GameController.Instance.itemOnMouse.quality == requiredQuality)
        {

			GameController.Instance.itemOnMouse.ReturnToBody ();
            
            Destroy(this.gameObject);
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
    }

    private string _tooltipText()
    {
        return "Required: " + requiredQuality;
    }
}
