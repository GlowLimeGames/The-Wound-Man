﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	public Room activeRoom;

    private static Vector3 playerOffset = new Vector3(-6.9f, -2.87f, 0.0f);

    private Camera _cam;

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
		//DontDestroyOnLoad(gameObject);
	}

	public void ChangeRoom (Room room) {
        PlayerController.Instance.transform.position = room.transform.position + playerOffset;

		Vector3 camDest = room.transform.position;
		camDest.z = -10.0f;
		_cam.transform.position = camDest;
	}

	// Use this for initialization
	void Start () {
		_cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
