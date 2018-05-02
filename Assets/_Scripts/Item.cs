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
        Long,
    };

    // TODO: This might change to a List<Quality>
    public Quality quality;

	public bool used;

    // Tooltip prefab
    public Transform tooltip;
    public Canvas canv;

	// "How far the object can slide in"
	// More accurately, the distance between the object's center and the point at which it's consdiered embedded.
    public float embeddedPartDistance;

	private Vector3 lastMousePos;

    public enum State
    {
        InRoom,
        InBody,

        OnMouse,

        Removing,
        Inserting
    }
    public State state;

    // A Tooltip prefab
	private Transform _activeTooltip;

	private Vector3 _mouseOffset;

    // Mask hiding the part of the item that's inside the body
	private SpriteMask _embeddedPart;
	private Vector3 _embeddedPartOriginalPosition;

	private Color _originalColor;

    // Guide arrow for extracting/inserting into body. Currently a child of it
    private Transform _arrow;
	private Vector3 _arrowOriginalScale;

    // Where the tooltip should spawn. Currently in the lower center of screen.
    private static Vector3 _tooltipPos = new Vector3(350, 75, 0);

    public void TakeFromRoom()
    {
        _EnablePlayerCollider();
        state = State.OnMouse;
 
        this.transform.SetParent(WoundMan.Instance.transform);
        WoundMan.Instance.inventory.Add(this);
    }

    public void TakeFromBody()
    {
        _EnablePlayerCollider();

        state = State.Removing;
        _showArrow(reverse: false);

        // Allow this to show up in death messages
		this.used = true;
    }

    public void ReturnToBody()
    {
        _RemoveFromMouse();
       
        state = State.Inserting;

        _showArrow(reverse: true);
        
    }


	// Use this for initialization
	void Start () {

		_embeddedPart = GetComponentInChildren<SpriteMask> ();
        _embeddedPartOriginalPosition = _embeddedPart.transform.localPosition;

		_originalColor = GetComponent<SpriteRenderer> ().color;

        // Need to adjust the spritemask so it doesn't look like it's stabbed into an invisible body
        if (state == State.InRoom)
        {
            Vector3 freePosition = _embeddedPart.transform.localPosition;
            freePosition.y -= embeddedPartDistance;
            _embeddedPart.transform.localPosition = freePosition;
        }
		
        _arrow = transform.Find("Arrow");
		_arrowOriginalScale = transform.localScale;

        _hideArrow();

		used = false;

        // Floats, between 1 and 10
        lethality = (Random.value * 9.0f) + 1.0f;
        efficiency = (Random.value * 9.0f) + 1.0f;

    }
	
	// Update is called once per frame
	void Update () {
        if (state == State.Removing) {
			// If the angle's close enough, slide it out for one frame

            // TODO: COuld just use a dot product and check if it's positive
			float ang = _angleToMouse();
			if (ang < 10.0f) {
				// Don't set the position directly, that might teleport it outside of the body
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 mouseDelta = mousePos - lastMousePos;

                // Move it in the direction of the item's up vector, but with the magnitude of the mouse's
                // movement along that vector.
                // a = mouseDelta, b = transform.up
                // To get the component of a along b, compute a dot b / |b|.
                float mouseMagnitudeAlongVector = Vector3.Dot(mouseDelta, transform.up) / transform.up.magnitude;
                Vector3 relativePosition = transform.up * mouseMagnitudeAlongVector;

                transform.position += relativePosition;
                _embeddedPart.transform.position += relativePosition * -1.0f; ;

				//print ((-1) * _embeddedPart.transform.localPosition.y + " " + embeddedPartDistance);

                if ((-1)*_embeddedPart.transform.localPosition.y > embeddedPartDistance)
                {
                    _FullyRemoveFromBody();
                }

				// You can put it back in if you change your mind.
				if ((_embeddedPart.transform.localPosition.y - 0.01f) > _embeddedPartOriginalPosition.y) {

					// Reset the position slighlty so it doesn't get permanently stuck.
					_embeddedPart.transform.localPosition = _embeddedPartOriginalPosition;
					_RemoveFromMouse ();
					_FullyInsertIntoBody ();
				}
			}
		}

		if (state == State.OnMouse)
        {
            // Item moves with the cursor
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos += _mouseOffset;
            mousePos.z = 0.0f;                // Otherwise it gets set to -10, and becomes invisible

            transform.position = mousePos;
        }

		if (state == State.Inserting) {
            float ang = _angleToMouse(reverse: true);
            if (ang < 10.0f) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Vector3 mouseDelta = mousePos - lastMousePos;

                float mouseMagnitudeAlongVector = Vector3.Dot(mouseDelta, transform.up) / transform.up.magnitude;
                Vector3 relativePosition = transform.up * mouseMagnitudeAlongVector;

                transform.position += relativePosition;
                // Still want the masking box to move opposite of mouseDelta.
                _embeddedPart.transform.position += relativePosition * -1.0f;

                if ((_embeddedPart.transform.localPosition.y) > _embeddedPartOriginalPosition.y)
                {
                    _FullyInsertIntoBody();
                }

				// You can take it back out if you change your mind
				if (Mathf.Abs (_embeddedPart.transform.localPosition.y) >
				    Mathf.Abs (_embeddedPartOriginalPosition.y) + embeddedPartDistance) {
					_EnablePlayerCollider ();
					_FullyRemoveFromBody ();

					// Place the embedded part a better distance away from it
					Vector3 freePosition = _embeddedPart.transform.localPosition;
					freePosition.y += embeddedPartDistance*0.5f;
					_embeddedPart.transform.localPosition = freePosition;
				}
            }
		}

		lastMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
	}

    void OnMouseDown()
    {
        if (state == State.InBody)
        {
			if (GameController.Instance.itemOnMouse == null) {
				TakeFromBody ();
			}
        } else if (state == State.InRoom)
        {
            TakeFromRoom();
            GameController.Instance.itemOnMouse = this;
        }
    }

	void OnMouseEnter() {

		if (_activeTooltip == null) {
			_activeTooltip = Instantiate (tooltip, _tooltipPos, Quaternion.identity);

            // Set tooltip text
            Text tooltipTextField = _activeTooltip.GetComponentInChildren<Text>();
            tooltipTextField.text = _tooltipText();

			_activeTooltip.SetParent (canv.transform, false);

            _activeTooltip.transform.position = _tooltipPos;
		}

		GetComponent<SpriteRenderer> ().color = Color.white;

	}

	void OnMouseExit() {

        if ((state == State.InBody) || (state == State.InRoom))
        {
			_destroyIfNotNull (_activeTooltip);
			GetComponent<SpriteRenderer> ().color = _originalColor;
        }


    }

    void OnDestroy()
    {
		_destroyIfNotNull (_activeTooltip);
    }

	private void _FullyRemoveFromBody() {
        state = State.OnMouse;
        GameController.Instance.itemOnMouse = this;

        // Ensure the object is held by the mouse at the spot where it was originally grabbed
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 itemPos = transform.position;
        _mouseOffset = itemPos - mousePos;

        _hideArrow();
		// TODO: Add a blood particle effect, or something to make it obvious
	}

	private void _FullyInsertIntoBody() {
        state = State.InBody;

        _hideArrow();
        //_embeddedPart.transform.localPosition = _embeddedPartOriginalPosition;
        GameController.Instance.itemOnMouse = null;

		_destroyIfNotNull (_activeTooltip);
	}

    private void _EnablePlayerCollider()
    {
        // Disable item collider and enable player collider.

		// TODO: Adjust the player boxcollider with the offset between mouse and item embed position.
		// Here's a start:
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 itemPos = transform.position;
		_mouseOffset = itemPos - mousePos;

        // Disable this collider, so you can have it on the mouse but still click things
        GetComponent<BoxCollider2D>().enabled = false;

        // Enable the player collider, so it can be clicked to return it to the body
        WoundMan.Instance.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void _RemoveFromMouse()
    {
        state = State.Inserting;

        // Enable its collider again so it can be clicked
        GetComponent<BoxCollider2D>().enabled = true;

        // Disable player collider, to allow items to be clicked instead
        WoundMan.Instance.GetComponent<BoxCollider2D>().enabled = false;
    }

	private float _angleToMouse(bool reverse=false) {
		float h = Input.GetAxis("Mouse X");
		float v = Input.GetAxis("Mouse Y");
		float mouseAngle = Vector3.Angle (Vector3.up, new Vector3(h, v));

        float itemAngle;

        if (reverse)
        {
            itemAngle = -transform.up.z;
        } else
        {
            itemAngle = transform.up.z;
        }

        return Mathf.DeltaAngle(mouseAngle, itemAngle);
	}

    private void _showArrow(bool reverse=false)
    {
		_arrow.localScale = _arrowOriginalScale;

        // If the item itself is flipped, the arrow must be flipped (again) too.
        if (transform.localScale.x < 0.0f)
        {
            reverse = !reverse;
        }

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
		_arrow.localScale = Vector3.zero;
    }

    private string _tooltipText()
    {
		// TODO: Any good way to style different parts of the text?
		// If "Rich Text" is enabled in the text component, you can use HTML tags to style it...
        return "<b>" + this.name + "</b>\n" + quality + "\n" + "Lethality: " + Mathf.Round(lethality).ToString() + "\n" + "Efficiency: " + Mathf.Round(efficiency).ToString();
    }

	private void _destroyIfNotNull(Transform t)
	{
		if (t != null) {
			Destroy (t.gameObject);
		}
	}
}
