using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Item itemOnMouse;
    public float animus;
    public Slider animusBar;
    public Text animusText;
    public GameObject deathScroll;

    public TextAsset deathText;
    
    public float animusBurnRate;

    private List<string> _deathTexts;

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

    public void DisplayDeath()
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

        // DEBUG
        animusBurnRate = 10.0f;
        _animusOfLastDeathNotification = 100.0f;

        _deathTexts = deathText.text.Split('\n').ToList();
	}
	
	// Update is called once per frame
	void Update () {
        // Burn and update animus value
        animus -= (animusBurnRate / 10) * Time.deltaTime;
        animusText.text = Mathf.Round(animus).ToString();
        animusBar.value = animus;

        // Display a death message if we're at 90, 80, 70...
        if (animus < _animusOfLastDeathNotification - 10.0f)
        {
            DisplayDeath();
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        // DEBUG
        //if (Input.GetKeyDown("d"))
        //{
        //    DisplayDeath();
        //}
	}

    private string _randomDeathText()
    {
        int index = Random.Range(0, _deathTexts.Count);
        string deathText = _deathTexts[index];
        //deathTexts.RemoveAt(index);     TODO: Remove the used ones once we have more strings
        return deathText;
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
