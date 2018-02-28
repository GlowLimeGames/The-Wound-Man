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

		//_embeddedPart.transform.localPosition = _embeddedPartOriginalPosition;

        // Put it back where it was
        // TODO: Might be more fun just to put it where clicked, so you can rearrange your inventory
		// (But would still use originalposition as a fallback, since it gets returned after solving a puzzle)
        //transform.position = _originalPos;
        //transform.localPosition = _originalLocalPos;

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

		used = false;

        lethality = Random.value * 10.0f;
        efficiency = Random.value * 10.0f;

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
				_embeddedPart.transform.position += (mouseDelta * -1.0f);

				// If it's all the way out of the body, put it on the mouse
				if (!_embeddedPart.bounds.Intersects (GetComponent<SpriteRenderer> ().bounds)) {
					_FullyRemoveFromBody ();
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

		// TODO: Not working yet
		if (_inserting) {
			print (_angleToMouse (reverse: true));
			if (_angleToMouse (reverse: true) < 10.0f) {
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Vector3 mouseDelta = mousePos - lastMousePos;
				transform.position += mouseDelta;

				_embeddedPart.transform.position += (mouseDelta);

				// TODO: I need another box or something to show how far it needs to be inserted.

				// TODO: Use positive masks instead of negative ones to avoid masking other items.

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
		// TODO: Add a blood particle effect, or something to make it obvious
	}

	private void _FullyInsertIntoBody() {
		_inserting = false;
	}

	private float _angleToMouse(bool reverse=false) {
		float h = Input.GetAxis("Mouse X");
		float v = Input.GetAxis("Mouse Y");
		float angleToUp = Vector3.Angle (Vector3.up, new Vector3(h, v));

		float angleDiff;
		float zAngle = transform.eulerAngles.z;

		if (zAngle < 180) {
			angleDiff = angleToUp - transform.rotation.eulerAngles.z;
		} else {
			angleDiff = angleToUp - (360.0f - transform.rotation.eulerAngles.z);
		}

		return Mathf.Abs(angleDiff);
	}

    private string _tooltipText()
    {
		// TODO: Any good way to style different parts of the text?
        return this.name + "\n" + quality + "\n" + "Lethality: " + Mathf.Round(lethality).ToString() + "\n" + "Efficiency: " + Mathf.Round(efficiency).ToString();
    }
}