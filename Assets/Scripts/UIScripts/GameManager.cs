using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool gameOver;
    public int gameStartScene;
    public int menuScene;
    public int enemyAmount;
    public float health;
    public float timer;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public TextMeshProUGUI winLoseText;
    public TextMeshProUGUI timerText;

    [SerializeField] static public bool isPaused = false;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject playerObject;

    private void Awake()
    {
        timer = 0;
    }
    private void Update()
    {
        if (gameOver == false)
        {
            timer += Time.deltaTime;
        }
        if (enemyAmount == 0 && gameOver == false)
        {
            gameOver = true;
            WinGame();
        }
        if (playerObject != null)
        {
            health = playerObject.GetComponent<IDamageable>().CurrentHP;
            if (health <= 0 && gameOver == false) 
            {
                gameOver = true;
                LoseGame();
            }
        }
        timerText.text = "" + timer;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);
        if (playerInput.inputIsActive == false)
        {
        playerInput.ActivateInput();

        }
        gameOver = false;
        gameOverMenu.SetActive(false);
        winLoseText.text = "";
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
        if (playerInput.inputIsActive == false)
        {
            playerInput.ActivateInput();

        }
        pauseMenu.SetActive(false);
    }
    public void WinGame()
    {
        timerText.text = "" + timer;
        StartCoroutine(GameEndWin());
    }
    public void LoseGame()
    {
        timerText.text = "" + timer;
        StartCoroutine(GameEndLose());
    }
    IEnumerator GameEndWin()
    {
        yield return new WaitForSeconds(1.5f);
        if (gameOverMenu != null)
        {
            isPaused = true;
            Time.timeScale = 0f;
            playerInput.DeactivateInput();
            gameOverMenu.SetActive(true);
            winLoseText.text = "CONGRATULATIONS!!!";
        }
    }
    IEnumerator GameEndLose()
    {
        yield return new WaitForSeconds(1.5f);
        if (gameOverMenu != null)
        {
            isPaused = true;
            Time.timeScale = 0f;
            playerInput.DeactivateInput();
            gameOverMenu.SetActive(true);
            winLoseText.text = "YOU ARE DEAD";
        }
    }
}
