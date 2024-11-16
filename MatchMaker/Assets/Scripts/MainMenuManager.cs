using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public List<Sprite> icons = new List<Sprite>();

    public Image icon1;
    public Image icon2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get a random index
        int randomIndex = Random.Range(0, icons.Count);
        icon1.sprite = icons[randomIndex];

        // Get a random index
        randomIndex = Random.Range(0, icons.Count);
        icon2.sprite = icons[randomIndex];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
