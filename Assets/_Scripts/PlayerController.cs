using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float _moveSpeed = 3.0f;
	private Vector3 _moveTarget;

	// Was the keyboard the last method used for movement input?
	private bool _keyboardLast;

	// Use this for initialization
	void Start () {
		_moveTarget = transform.position;
		_keyboardLast = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetAxis ("Horizontal") != 0.0f) {
			_keyboardLast = true;
			var x = Input.GetAxis ("Horizontal") * Time.deltaTime * _moveSpeed;
			transform.Translate (x, 0, 0);

		} else {

			if (Input.GetMouseButtonDown (0)) {
				_keyboardLast = false;
				Camera cam = Camera.main;
				_moveTarget = cam.ScreenToWorldPoint (Input.mousePosition);
				_moveTarget.y = transform.position.y;   // Don't move vertically at all
				_moveTarget.z = transform.position.z;

			}

			float _targetDelta = _moveTarget.x - transform.position.x;

			// Only move toward the mouse target if the keyboard's not being used now
			if (!_keyboardLast) {
				if (Mathf.Abs (_targetDelta) > 0.5f) {
					transform.position = Vector3.MoveTowards (transform.position, _moveTarget, _moveSpeed * Time.deltaTime);
				} else {
					transform.position = transform.position;
				}
			}

		}

	}
}
