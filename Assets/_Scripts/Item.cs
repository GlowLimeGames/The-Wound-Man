using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

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

	public bool used;

    // Tooltip prefab
    public Transform tooltip;
    public Canvas canv;

    public float embeddedPartLength;

	private Vector3 lastMousePos;

	private Transform _activeTooltip;
    private bool _onMouse;
	private bool _removing;
	private bool _inserting;
	private Vector3 _mouseOffset;

    private Vector3 _originalPos;
    private Vector3 _originalLocalPos;

	private SpriteMask _embeddedPart;
	private Vector3 _embeddedPartOriginalPosition;

    private Transform _arrow;

    private static Vector3 _tooltipPos = new Vector3(0, 250, 0);

	public void Use()
	{
		// Deal animus damage.
		// TODO: Is it a random chance to get decreased by lethality, or does it always decrease? 
		// Going with constant decrease for now
		//print("Set burn rate");
		GameController.Instance.animusBurnRate = lethality;
	}

	public void DoneUsing()
	{
		GameController.Instance.animusBurnRate = 0.0f;
	}

    public void TakeFromBody()
    {
        //_onMouse = true;
		_removing = true;

        _showArrow(reverse: false);

        GameController.Instance.itemOnMouse = this;

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 itemPos = transform.position;
		_mouseOffset = itemPos - mousePos;

        // Allow this to show up in death messages
		this.used = true;

        // Disable this collider, so you can have it on the mouse but still click things
        GetComponent<BoxCollider2D>().enabled = false;

        // Enable the player collider, so it can be clicked to return it to the body
        WoundMan.Instance.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ReturnToBody()
    {
        _onMouse = false;

		// TODO: redundant?
		_removing = false;

		_inserting = true;

        _showArrow(reverse: true);
        
        // Enable its collider again so it can be clicked
        GetComponent<BoxCollider2D>().enabled = true;

        // Disable player collider, to allow items to be clicked instead
        WoundMan.Instance.GetComponent<BoxCollider2D>().enabled = false;

        //GameController.Instance.itemOnMouse = null;
        //Destroy(_activeTooltip.gameObject);

        //GameController.Instance.animusBurnRate -= lethality;
    }

	// Use this for initialization
	void Start () {
        _onMouse = false;
        _originalPos = transform.position;
        _originalLocalPos = transform.localPosition;

		_embeddedPart = GetComponentInChildren<SpriteMask> ();
		_embeddedPartOriginalPosition = _embeddedPart.transform.localPosition;

        _arrow = transform.Find("Arrow");

        _hideArrow();

		used = false;

        // Floats between 1 and 10
        lethality = (Random.value * 9.0f) + 1.0f;
        efficiency = (Random.value * 9.0f) + 1.0f;

    }
	
	// Update is called once per frame
	void Update () {
		if (_removing) {
			// If the angle's close enough, slide it out for one frame
			// TODO: Should you also be able to slide it back in?
			float ang = _angleToMouse();
			if (ang < 10.0f) {
				// Don't set the position directly, that might teleport it outside of the body
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 mouseDelta = mousePos - lastMousePos;
				transform.position += mouseDelta;

				// Slide spritemask that far away
                // TODO: To avoid the kinda rough "diagonal sliding" thing happening, it might be
                //       better to slide it only along the correct removal axis, not with the full mouseDelta
				_embeddedPart.transform.position += (mouseDelta * -1.0f);

                // If it's all the way out of the body, put it on the mouse
                //if (!_embeddedPart.bounds.Intersects (GetComponent<SpriteRenderer> ().bounds)) {
                //	_FullyRemoveFromBody ();
                //}

                if ((-1)*_embeddedPart.transform.localPosition.y > embeddedPartLength)
                {
                    _FullyRemoveFromBody();
                }

                
			}
		}

		if (_onMouse)
        {
            // Item moves with the cursor
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos += _mouseOffset;
            mousePos.z = 0.0f;                // Otherwise it gets set to -10, and becomes invisible

            transform.position = mousePos;
        }

		if (_inserting) {
            // TODO: Still happening instantly for the hammer. Not sure why
            if (_angleToMouse (reverse: true) < 10.0f) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Vector3 mouseDelta = mousePos - lastMousePos;
				transform.position += mouseDelta;

                // Still want the masking box to move opposite of mouseDelta.
				_embeddedPart.transform.position += (mouseDelta * -1.0f);

                if ((_embeddedPart.transform.localPosition.y) > _embeddedPartOriginalPosition.y)
                {
                    _FullyInsertIntoBody();
                }

            }
		}

		lastMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
	}

    void OnMouseDown()
    {
        if (!_onMouse)
        {
            TakeFromBody();

        }
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
		if ((!_onMouse) && (!_removing))
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

	private void _FullyRemoveFromBody() {
		_removing = false;
		_onMouse = true;

        _arrow.localScale = Vector3.zero;
		// TODO: Add a blood particle effect, or something to make it obvious
	}

	private void _FullyInsertIntoBody() {
		_inserting = false;

        _arrow.localScale = Vector3.zero;
        //_embeddedPart.transform.localPosition = _embeddedPartOriginalPosition;
        GameController.Instance.itemOnMouse = null;
	}

	private float _angleToMouse(bool reverse=false) {
		float h = Input.GetAxis("Mouse X");
		float v = Input.GetAxis("Mouse Y");
		float mouseAngle = Vector3.Angle (Vector3.up, new Vector3(h, v));

		float angleDiff;
		float zAngle = transform.eulerAngles.z;

        if (reverse)
        {
            zAngle = zAngle - 180.0f;
        }

		if (zAngle < 180) {
			angleDiff = mouseAngle - zAngle;
		} else {
            angleDiff = mouseAngle - (360.0f - zAngle);
            //angleDiff = mouseAngle - transform.rotation.eulerAngles.z - 180.0f;
		}

		return Mathf.Abs(angleDiff);
	}

    private void _showArrow(bool reverse=false)
    {
        _arrow.localScale = Vector3.one * 0.25f;

        if (reverse)
        {
            _arrow.localEulerAngles = new Vector3(0, 0, 90);
        } else
        {
            _arrow.localEulerAngles = new Vector3(0, 0, 270);
        }
    }

    private void _hideArrow()
    {
        _arrow.localScale = new Vector3(0, 0, 0);
    }

    private string _tooltipText()
    {
		// TODO: Any good way to style different parts of the text?
        return this.name + "\n" + quality + "\n" + "Lethality: " + Mathf.Round(lethality).ToString() + "\n" + "Efficiency: " + Mathf.Round(efficiency).ToString();
    }
}