using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour {

	public Room destinationRoom;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseUp() {
        // Go to the destination room only if not holding an item
        if (GameController.Instance.itemOnMouse == null)
        {
            Map.Instance.ChangeRoom(destinationRoom);
        }
		
	}
}
