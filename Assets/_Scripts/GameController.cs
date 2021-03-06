﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public Item itemOnMouse;
    public float animus;
    public Slider animusBar;
    public Text animusText;
    public Text roomNameText;
    public GameObject deathScroll;

    public TextAsset deathText;
    public TextAsset firstNameText;
    public TextAsset lastNameText;

    public static float ANIMUS_DAMAGE_PER_CLICK = 5.0f;

    private List<string> _deathTexts;
    private List<string> _firstNames;
    private List<string> _lastNames;

    private float _animusOfLastDeathNotification;

    private Color _originalScrollColor;

    private bool _fadingOut;

    // Singleton. Can access as GameController.Instance
    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }
    private static GameController instance = null;

	public void AnimusDamage(float d)
	{
		animus -= d;

		animusText.text = Mathf.Round(animus).ToString();
		animusBar.value = animus;

		// Display a death message if we're at 90, 80, 70...
		if (animus <= _animusOfLastDeathNotification - 10.0f)
		{
			DisplayDeath();
		}

        if (animus <= 0.0f)
        {
            SceneManager.LoadScene("GameOverScreen");
        }
	}

    public void DisplayDeath()
    {
        string victim = _randomFirstName() + " " + _randomLastName();
        string death = _randomDeathText();
        string text;

        if (death.StartsWith(","))
        {
            text = victim + death;
        } else
        {
            text = victim + " " + death;
        }
			
        // TODO: This replaces them all. Need to replace them one by one...
        while (text.Contains("First Name"))
        {
            text = _replaceFirst(text, "First Name", _randomFirstName());
        }

        while (text.Contains("Last Name"))
        {
            text = _replaceFirst(text, "Last Name", _randomLastName());
        }

        deathScroll.GetComponentInChildren<Text>().text = text;
        _animusOfLastDeathNotification = animus;

        // Death scroll starts inactive, so make sure it is active now
        deathScroll.SetActive(true);

        StartCoroutine("FadeOutDeathText");
        StartCoroutine("HideDeathText");
    }

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

        _animusOfLastDeathNotification = 100.0f;

        _deathTexts = deathText.text.Split('\n').ToList();
        _firstNames = firstNameText.text.Split('\n').ToList();
        _lastNames = lastNameText.text.Split('\n').ToList();

        _originalScrollColor = deathScroll.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        // DEBUG
        if (Input.GetKeyDown("d"))
        {
            DisplayDeath();
        }

        if (_fadingOut)
        {
            Color c = deathScroll.GetComponent<Image>().color;
            c.a -= 1 * Time.deltaTime;
            deathScroll.GetComponent<Image>().color = c;
        }
	}

    IEnumerator FadeOutDeathText()
    {
        yield return new WaitForSeconds(12);
        _fadingOut = true;
    }

    IEnumerator HideDeathText()
    {
        yield return new WaitForSeconds(13);
        _fadingOut = false;
        deathScroll.GetComponent<Image>().color = _originalScrollColor;
        deathScroll.SetActive(false);

    }

    private string _replaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    private string _randomDeathText()
    {
        int index = Random.Range(0, _deathTexts.Count);
        string deathText = _deathTexts[index];
        //deathTexts.RemoveAt(index);     TODO: Remove the used ones once we have more strings
        return deathText;
    }

    private string _randomFirstName()
    {
        int index = Random.Range(0, _firstNames.Count);
        return _firstNames[index];
    }

    private string _randomLastName()
    {
        int index = Random.Range(0, _lastNames.Count);
        return _lastNames[index];
    }

    private string _randomUsedItemName()
    {
        // Make sure at least one item has been used to avoid infinite loop
        if (!_atLeastOneItemUsed())
        {
            // Just use a placeholder if not
            return "Sword";
        }

        List<Item> inventory = WoundMan.Instance.inventory;

		int index = Random.Range(0, inventory.Count);
		while (!inventory [index].used) {
			index = Random.Range (0, inventory.Count);
		}
		return inventory [index].name;
    }

    private bool _atLeastOneItemUsed()
    {
        // Make sure at least one item has been used so far
        List<Item> inventory = WoundMan.Instance.inventory;
        foreach (var item in inventory)
        {
            if (item.used)
            {
                return true;
            }
        }
        return false;
    }
}
