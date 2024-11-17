using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public List<Sprite> icons = new List<Sprite>();

    public Image icon1;
    public Image icon2;

    public GameObject menuUI;
    public GameObject htpUI;
    public GameObject playUI;

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

    public void OnPlayClicked()
    {
        menuUI.SetActive(false);
        playUI.SetActive(true);
    }

    public void OnNormalClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnSpeedClicked()
    {
        SceneManager.LoadScene(2);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnHTPClicked()
    {
        menuUI.SetActive(false);
        htpUI.SetActive(true);
    }

    public void OnReturnClicked()
    {
        menuUI.SetActive(true);
        htpUI.SetActive(false);
        playUI.SetActive(false);
    }
}
