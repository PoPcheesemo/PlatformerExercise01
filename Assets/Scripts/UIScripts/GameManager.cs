using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int gameStartScene;
    public int menuScene;
    [SerializeField] static public bool isPaused = false;
    public GameObject pauseMenu;

    [SerializeField] private PlayerInput playerInput;

    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(menuScene);
    }
    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isPaused)
            {
                Debug.Log("UNPAUSE");
                UnPause();
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0f;
                playerInput.DeactivateInput();
                pauseMenu.SetActive(true);
            }
        }
    }
    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        playerInput.ActivateInput();
        pauseMenu.SetActive(false);
    }
}
