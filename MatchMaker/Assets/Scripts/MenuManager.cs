using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseObj;
    public GameObject gameUIObj;

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
}
