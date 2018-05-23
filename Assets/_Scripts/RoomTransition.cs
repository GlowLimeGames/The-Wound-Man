using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour {

	public Room destinationRoom;

    private Color _originalColor;

	// Use this for initialization
	void Start () {
        _originalColor = GetComponent<SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = _originalColor;
    }

	void OnMouseUp() {
        // Go to the destination room only if not holding an item
        if (GameController.Instance.itemOnMouse == null)
        {
            Map.Instance.ChangeRoom(destinationRoom);
        }
		
	}
}
