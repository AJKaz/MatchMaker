using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseObj;
    public GameObject gameUIObj;

    public GameObject speedDatingObj;

    public GameObject timerObj;

    public GameObject winObj;
    public GameObject lossObj;

    public bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.Escape) || !isPaused && Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        else if (isPaused && Input.GetKeyDown(KeyCode.Escape) || isPaused && Input.GetKeyDown(KeyCode.P))
        {
            UnpauseGame();
        }
    }

    public void PauseGame()
    {
        pauseObj.SetActive(true);
        isPaused = true;
    }

    public void UnpauseGame()
    {
        pauseObj.SetActive(false);
        isPaused = false;
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
    public void SpeedDatingMenu()
    {
        speedDatingObj.SetActive(true);
        gameUIObj.SetActive(false);
        timerObj.SetActive(false) ;
    }

    public void OnResumeClick()
    {
        UnpauseGame();
    }

    public void OnExitToMenuClick()
    {
        SceneManager.LoadScene(0);
    }

    public void OnReplayNormalClick()
    {
        SceneManager.LoadScene(1);
    }

    public void OnReplaySpeedClick()
    {
        SceneManager.LoadScene(2);
    }
}
