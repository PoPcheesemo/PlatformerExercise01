using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int gameStartScene;
    public int menuScene;
    static public bool isPaused;

    [SerializeField] private PlayerInput playerInput;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
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
                UnPause();
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0f;
                playerInput.enabled = false;
            }
        }
    }
    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        playerInput.enabled = true;
    }
}
