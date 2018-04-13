using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public Room activeRoom;

    private static Vector3 playerOffset = new Vector3(-6.8f, -2.72f, 0.0f);

	// Make this a singleton. Can access it from objects with Map.Instance
	public static Map Instance
	{
		get
		{
			return instance;
		}
	}
	private static Map instance = null;

	void Awake()
	{
		if (instance)
		{
			DestroyImmediate(gameObject);
			return;
		}
		instance = this;
	}

	public void ChangeRoom (Room room) {
		room.gameObject.SetActive (true);

		// Place character and camera in new room
        WoundMan.Instance.transform.position = room.transform.position + playerOffset;
		Vector3 camDest = room.transform.position;
		camDest.z = -10.0f;
		Camera.main.transform.position = camDest;

		// Deactivate previous room once it's offscreen
		activeRoom.gameObject.SetActive (false);

		// Update activeRoom
		activeRoom = room;

        GameController.Instance.roomNameText.text = room.name;
	}

	// Use this for initialization
	void Start () {
		// Deactivate all non-current rooms
		Room[] rooms = (Room[]) GameObject.FindObjectsOfType (typeof(Room));
		foreach (Room room in rooms) {
			if (room != activeRoom) {
				room.gameObject.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
