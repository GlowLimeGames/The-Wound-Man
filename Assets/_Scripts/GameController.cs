using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public ItemController itemOnMouse;
    public float animus;
    public Text animusText;

    public float animusBurnRate;

	// Use this for initialization
	void Start () {
        animus = 100.0f;
        animusBurnRate = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
        animus -= (animusBurnRate / 10) * Time.deltaTime;
        animusText.text = "Animus: " + Mathf.Round(animus).ToString();
	}
}
