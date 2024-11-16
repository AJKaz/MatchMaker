using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseObj;
    public GameObject gameUIObj;

    public GameObject winObj;
    public GameObject lossObj;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseObj.SetActive(true);
        gameUIObj.SetActive(true);
    }

    public void UnpauseGame()
    {
        pauseObj.SetActive(false);
        gameUIObj.SetActive(false);
    }

    public void WinMenu()
    {
        winObj.SetActive(true);
        gameUIObj.SetActive(false);
    }

    public void LossMenu()
    {
        lossObj.SetActive(true);
        gameUIObj.SetActive(false);
    }
}
