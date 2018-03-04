﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Item itemOnMouse;
    public float animus;
    public Text animusText;
    public GameObject deathScroll;

    public List<string> deathTexts;

    public float animusBurnRate;

    private float _animusOfLastDeathNotification;

    // Singleton. Can access as GameController.Instance
    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }
    private static GameController instance = null;

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

    // Use this for initialization
    void Start () {
        animus = 100.0f;
        animusBurnRate = 0.0f;
        _animusOfLastDeathNotification = 100.0f;
	}
	
	// Update is called once per frame
	void Update () {
        // Burn and update animus value
        animus -= (animusBurnRate / 10) * Time.deltaTime;
        animusText.text = "Animus: " + Mathf.Round(animus).ToString();

        // Display a death message if we're at 90, 80, 70...
        if (animus < _animusOfLastDeathNotification - 10.0f)
        {
            string text = _randomDeathText();
            string itemName = _randomUsedItemName();
            // TODO: We can do other replacements eventually, like random names/locations as well
            text = text.Replace("{ITEM}", itemName);
            deathScroll.GetComponentInChildren<Text>().text = text;
            _animusOfLastDeathNotification = animus;

            // Death scroll starts inactive
            deathScroll.SetActive(true);
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
	}

    private string _randomDeathText()
    {
        int index = Random.Range(0, deathTexts.Count);
        string deathText = deathTexts[index];
        //deathTexts.RemoveAt(index);     TODO: Remove the used ones once we have more strings
        return deathText;
    }

    private string _randomUsedItemName()
    {
        // TODO: Make sure at least one item has been used to avoid infinite loop
        List<Item> inventory = WoundMan.Instance.inventory;

		int index = Random.Range(0, inventory.Count);
		while (!inventory [index].used) {
			index = Random.Range (0, inventory.Count);
		}
		return inventory [index].name;
    }
}
